using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Ordering;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Routing;

namespace TransportSystems.Backend.Application.Business.Ordering
{
    public class ApplicationOrderValidatorService : IApplicationOrderValidatorService
    {
        public ApplicationOrderValidatorService(
            IApplicationRouteService routeService)
        {
            RouteService = routeService;
        }

        protected IApplicationRouteService RouteService { get; }

        public Task Validate(BookingAM booking, RouteAM orderRoute, BillAM orderBill)
        {
            if (!booking.Bill.TotalCost.Equals(orderBill.TotalCost))
            {
                throw new ValidationException($"TotalCost in the bookingBill: {booking.Bill.TotalCost} doesn't equal from the orderBill: {orderBill.TotalCost}");
            }

            var orderDistance = RouteService.GetTotalDistance(orderRoute);
            if (!booking.Bill.Basket.Distance.Equals(orderDistance))
            {
                throw new ValidationException($"TotalDistance in the bookingBasket: {booking.Bill.Basket.Distance} doesn't equal from the orderRoute: {orderDistance}");
            }

            return Task.CompletedTask;
        }
    }
}