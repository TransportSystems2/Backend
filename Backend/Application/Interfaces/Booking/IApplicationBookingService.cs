using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.Application.Interfaces.Booking
{
    public interface IApplicationBookingService : IApplicationBaseService
    {
        Task<BookingResponseAM> CalculateBooking(BookingRequestAM request);

        Task<BookingRouteAM> CalculateBookingRoute(RouteAM route, CargoAM cargo, BasketAM requestBasket);
    }
}