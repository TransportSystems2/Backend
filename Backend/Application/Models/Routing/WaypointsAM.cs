using System.Collections.Generic;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Models.Routing
{
    public class WaypointsAM : BaseAM
    {
        public List<AddressAM> Points { get; set; }

        public string Comment { get; set; }
    }
}