using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Models.Routing
{
    public class RouteLegAM : BaseAM
    {
        public AddressAM StartAddress { get; set; }

        public AddressAM EndAddress { get; set; }

        public int Duration { get; set; }

        public int Distance { get; set; }

        public RouteLegKind Kind { get; set; }
    }
}