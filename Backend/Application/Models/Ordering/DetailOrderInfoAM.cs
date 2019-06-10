using TransportSystems.Backend.Application.Models;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Ordering;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Models.Ordering
{
    public class DetailOrderInfoAM : OrderInfoAM
    {
        public DetailOrderInfoAM()
            : this(null)
        {
        }

        public DetailOrderInfoAM(OrderInfoAM info)
            : base(info)
        {
        }

        public CustomerAM Customer { get; set; }

        public CargoAM Cargo { get; set; }

        public RouteAM Route { get; set; }

        public BillAM Bill { get; set; }
    }
}