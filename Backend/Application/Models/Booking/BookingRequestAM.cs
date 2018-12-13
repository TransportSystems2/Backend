using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.Application.Models.Booking
{
    public class BookingRequestAM : BaseAM
    {
        public WaypointsAM Waypoints { get; set; }

        public BasketAM Basket { get; set; }

        public CargoAM Cargo { get; set; }
    }
}