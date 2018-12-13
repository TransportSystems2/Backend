using Common.Models.Geolocation;

namespace TransportSystems.Backend.External.Models.Routing
{
    public class LegEM : BaseEM
    {
        public Coordinate StartCoordinate { get; set; }

        public Coordinate EndCoordinate { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// In meter
        /// </summary>
        public int Distance { get; set; }
    }
}