using System;

namespace TransportSystems.Backend.Core.Domain.Core.Routing
{
    /// <summary>
    /// Leg of a route
    /// </summary>
    public enum RouteLegKind
    {
        /// <summary>
        /// Feeding a vhicle to client (from garage)
        /// </summary>
        Feed = 0,

        /// <summary>
        /// Transportation of client cargo
        /// </summary>
        Transportation = 1,

        /// <summary>
        /// Way back to garage
        /// </summary>
        WayBack = 2
    }

    public class RouteLeg : BaseEntity
    {
        public int RouteId { get; set; }

        public int StartAddressId { get; set; }

        public int EndAddressId { get; set; }

        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Metres
        /// </summary>
        public int Distance { get; set; }

        public RouteLegKind Kind { get; set; }
    }
}