using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Interfaces.Prognosing;

namespace TransportSystems.Backend.Application.Business.Booking
{
    public class ApplicationBookingService : ApplicationBaseService, IApplicationBookingService
    {
        public ApplicationBookingService(
                IApplicationAddressService addressService,
                IApplicationBillService billService,
                IApplicationRouteService routeService,
                IApplicationPrognosisService prognosisService)
        {
            AddressService = addressService;
            BillService = billService;
            RouteService = routeService;
            PrognosisService = prognosisService;
        }

        protected IApplicationAddressService AddressService { get; }

        protected IApplicationBillService BillService { get; }

        protected IApplicationRouteService RouteService { get; }

        protected IApplicationPrognosisService PrognosisService { get; }

        public async Task<BookingResponseAM> CalculateBooking(BookingRequestAM request)
        {
            var result = new BookingResponseAM();

            var firstWaypointCoordinate = request.Waypoints.Points.First().ToCoordinate();
            var rootAddresses = await AddressService.GetNearestAddresses(AddressKind.City, firstWaypointCoordinate);

            var possibleRoutes = await RouteService.GetPossibleRoutes(rootAddresses, request.Waypoints);
            var bookingRoutes = await GetBookingRoutes(possibleRoutes, request.Cargo, request.Basket);
            bookingRoutes = bookingRoutes.OrderBy(b => b.Bill.TotalCost).ToList();

            result.Routes.AddRange(bookingRoutes);

            return result;
        }

        public async Task<BookingRouteAM> CalculateBookingRoute(RouteAM route, CargoAM cargo, BasketAM requestBasket)
        {
            var rootAddress = RouteService.GetRootAddress(route);
            var rootAddressTimeBelt = await AddressService.GetTimeBeltByAddress(rootAddress);
            var feedDistance = RouteService.GetFeedDistance(route);
            var totalDistance = RouteService.GetTotalDistance(route);
            var avgDeliveryTime = await PrognosisService.GetAvgDeliveryTime(route, cargo, requestBasket);

            var billInfo = await BillService.GetBillInfo(rootAddress.ToCoordinate(), cargo.WeightCatalogItemId);

            // клонируем т.к. количество километров для каждого маршрута - индивидуально, а параметром передается общий объект
            var basket = requestBasket.Clone() as BasketAM;
            basket.Distance = totalDistance;

            var bill = await BillService.CalculateBill(billInfo, basket);
            var title = $"{rootAddress.Locality} - {bill.TotalCost}₽";

            return new BookingRouteAM
            {
                RootAddress = rootAddress,
                RootAddressTimeBelt = rootAddressTimeBelt,
                FeedDistance = feedDistance,
                TotalDistance = totalDistance,
                Bill = bill,
                Title = title,
                AvgDeliveryTime = avgDeliveryTime
            };
        }

        private async Task<ICollection<BookingRouteAM>> GetBookingRoutes(ICollection<RouteAM> possibleRoutes, CargoAM cargo, BasketAM basket)
        {
            var result = new ConcurrentBag<BookingRouteAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            await possibleRoutes.ParallelForEachAsync(
                async route =>
                {
                    try
                    {
                        var bookingRoute = await CalculateBookingRoute(route, cargo, basket);
                        result.Add(bookingRoute);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                }
            );

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return result.ToList();
        }
    }
}