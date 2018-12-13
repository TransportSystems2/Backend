using System;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Models.Booking
{
    public class BookingAM : BaseAM
    {
        public AddressAM RootAddress { get; set; }

        public WaypointsAM Waypoints { get; set; }

        public DateTime OrderTime { get; set; }

        public CustomerAM Customer {get;set;}

        public CargoAM Cargo { get; set; }

        public BillInfoAM BillInfo { get; set; }

        public BasketAM Basket { get; set; }
    }
}