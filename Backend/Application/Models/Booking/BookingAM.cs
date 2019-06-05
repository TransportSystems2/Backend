using System;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Models.Booking
{
    public class BookingAM : BaseAM
    {
        public int MarketId { get; set; }

        public WaypointsAM Waypoints { get; set; }

        public DateTime TimeOfDelivery { get; set; }

        public CustomerAM Customer { get; set; } 

        public CargoAM Cargo { get; set; }

        public BillAM Bill { get; set; }
    }
}