using System.Collections.Generic;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Models.Routing
{
    public class RouteEM : BaseEM
    {
        public RouteEM()
        {
            Legs = new List<LegEM>();
        }

        public List<LegEM> Legs { get; set; }

        public Status Status { get; set; }
    }
}