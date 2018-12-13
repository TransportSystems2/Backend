using System.Collections.Generic;

namespace TransportSystems.Backend.Application.Models.Routing
{
    public class RouteAM : BaseAM
    {
        public RouteAM()
        {
            Legs = new List<RouteLegAM>();
        }

        public List<RouteLegAM> Legs { get; }

        public string Comment { get; set; }
    }
}