using DotNetDistance;
using System;

namespace TransportSystems.Backend.Core.Domain.Core.Routing
{
    /// <summary>
    /// Leg of a route
    /// </summary>
    [Flags]
    public enum RouteLegKind
    {
        /// <summary>
        /// Feeding a vhicle to client (from garage)
        /// </summary>
        Feed = 1,

        /// <summary>
        /// Transportation of client cargo
        /// </summary>
        Transportation = 2,

        /// <summary>
        /// Way back to garage
        /// </summary>
        WayBack = 4,

        /// <summary>
        /// All
        /// </summary>
        All = Feed | Transportation | WayBack
    }

    public class RouteLeg : BaseEntity
    {
        public int RouteId { get; set; }

        public int StartAddressId { get; set; }

        public int EndAddressId { get; set; }

        public TimeSpan Duration { get; set; }

        public Distance Distance { get; set; }

        public RouteLegKind Kind { get; set; }
    }
}