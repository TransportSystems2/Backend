using System;

namespace TransportSystems.Backend.Core.Domain.Core.Transport
{
    [Flags]
    public enum VehicleKind
    {
        /// <summary>
        /// Ломаная платформа, с лебедкой
        /// </summary>
        BrokenPlatform = 0,

        /// <summary>
        /// Сдвижная платформа
        /// </summary>
        SlidingPlatform = 1,

        /// <summary>
        /// Кран манипулятор
        /// </summary>
        CraneManipulator = 2
    }
}