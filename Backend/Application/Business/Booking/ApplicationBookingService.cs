using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Prognosing;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Application.Business.Booking
{
    public class ApplicationBookingService : ApplicationBaseService, IApplicationBookingService
    {
        public ApplicationBookingService(
                IApplicationAddressService addressService,
                IApplicationBillService billService,
                IApplicationRouteService routeService,
                IApplicationPrognosisService prognosisService,
                IApplicationMarketService marketService,
                IApplicationUserService userService)
        {
            AddressService = addressService;
            BillService = billService;
            RouteService = routeService;
            PrognosisService = prognosisService;
            MarketService = marketService;
            UserService = userService;
        }

        protected IApplicationAddressService AddressService { get; }

        protected IApplicationBillService BillService { get; }

        protected IApplicationRouteService RouteService { get; }

        protected IApplicationPrognosisService PrognosisService { get; }

        protected IApplicationMarketService MarketService { get; }

        protected IApplicationUserService UserService { get; }

        public async Task<BookingResponseAM> CalculateBooking(int identityUserId, BookingRequestAM request)
        {
            var result = new BookingResponseAM();

            var firstWaypointCoordinate = request.Waypoints.Points.First().ToCoordinate();
            var domainDispatcher = await UserService.GetDomainDispatcherByIdentityUser(identityUserId); 
            var markets = await MarketService.GetNearestDomainMarkets(domainDispatcher.CompanyId, firstWaypointCoordinate);

            var bookingRoutes = await GetBookingRoutes(markets, request.Waypoints, request.Cargo, request.Basket);
            bookingRoutes = bookingRoutes.OrderBy(b => b.Bill.TotalCost).ToList();

            result.Routes.AddRange(bookingRoutes);

            return result;
        }

        public async Task<BookingRouteAM> CalculateBookingRoute(
            Market market,
            WaypointsAM waypoints,
            CargoAM cargo,
            BasketAM requestBasket)
        {
            var marketAddress = await AddressService.GetAddress(market.AddressId);
            var route = await RouteService.FindRoute(marketAddress, waypoints);

            var rootAddress = RouteService.GetRootAddress(route);
            var marketTimeBelt = await AddressService.GetTimeBeltByAddress(rootAddress);
            var feedDistance = RouteService.GetFeedDistance(route);
            var totalDistance = RouteService.GetTotalDistance(route);
            var avgDeliveryTime = await PrognosisService.GetAvgDeliveryTime(route, cargo, requestBasket);

            var billInfo = await BillService.GetDefaultBillInfo(market.PricelistId, cargo.WeightCatalogItemId);

            // клонируем т.к. количество километров для каждого маршрута - индивидуально, а параметром передается общий объект
            var basket = requestBasket.Clone() as BasketAM;
            basket.Distance = totalDistance;

            var bill = await BillService.CalculateBill(billInfo, basket);
            var title = $"{rootAddress.Locality} - {bill.TotalCost}₽";

            return new BookingRouteAM
            {
                MarketId = market.Id,
                MarketTimeBelt = marketTimeBelt,
                FeedDistance = feedDistance,
                TotalDistance = totalDistance,
                Bill = bill,
                Title = title,
                AvgDeliveryTime = avgDeliveryTime
            };
        }

        private async Task<ICollection<BookingRouteAM>> GetBookingRoutes(
            ICollection<Market> markets,
            WaypointsAM wayPoints,
            CargoAM cargo,
            BasketAM basket)
        {
            var result = new ConcurrentBag<BookingRouteAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            await markets.ParallelForEachAsync(
                async market =>
                {
                    try
                    {
                        var bookingRoute = await CalculateBookingRoute(market, wayPoints, cargo, basket);
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