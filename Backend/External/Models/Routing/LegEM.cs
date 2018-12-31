using Common.Models.Units;
using System;

namespace TransportSystems.Backend.External.Models.Routing
{
    public class LegEM : BaseEM
    {
        public Coordinate StartCoordinate { get; set; }

        public Coordinate EndCoordinate { get; set; }

        public TimeSpan Duration { get; set; }

        /// <summary>
        /// In meter
        /// </summary>
        public int Distance { get; set; }
    }
}