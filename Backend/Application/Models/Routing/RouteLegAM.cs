using DotNetDistance;
using System;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Core.Domain.Core.Routing;

namespace TransportSystems.Backend.Application.Models.Routing
{
    public class RouteLegAM : BaseAM
    {
        public AddressAM StartAddress { get; set; }

        public AddressAM EndAddress { get; set; }

        public TimeSpan Duration { get; set; }

        public Distance Distance { get; set; }

        public RouteLegKind Kind { get; set; }
    }
}