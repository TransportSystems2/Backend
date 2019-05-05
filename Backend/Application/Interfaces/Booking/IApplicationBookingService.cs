using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Application.Interfaces.Booking
{
    public interface IApplicationBookingService : IApplicationBaseService
    {
        Task<BookingResponseAM> CalculateBooking(int identityUserId, BookingRequestAM request);

        Task<BookingRouteAM> CalculateBookingRoute(Market market, WaypointsAM waypoints, CargoAM cargo, BasketAM requestBasket);
    }
}