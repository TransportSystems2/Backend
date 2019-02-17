using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Routing;

namespace TransportSystems.Backend.Application.Interfaces.Ordering
{
    public interface IApplicationOrderValidatorService : IApplicationBaseService
    {
        Task Validate(BookingAM booking, RouteAM orderRoute, BillAM orderBill);
    }
}