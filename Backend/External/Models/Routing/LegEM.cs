using Common.Models.Units;
using DotNetDistance;
using System;

namespace TransportSystems.Backend.External.Models.Routing
{
    public class LegEM : BaseEM
    {
        public Coordinate StartCoordinate { get; set; }

        public Coordinate EndCoordinate { get; set; }

        public TimeSpan Duration { get; set; }

        public Distance Distance { get; set; }
    }
}