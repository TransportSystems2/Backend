using DotNetDistance;
using System;

namespace TransportSystems.Backend.Application.Models.Billing
{
    public class BasketAM : BaseAM, ICloneable
    {
        /// <summary>
        /// Протяженость маршрута
        /// </summary>
        public Distance Distance { get; set; }

        /// <summary>
        /// Требуется погрузка
        /// </summary>
        public int LoadingValue { get; set; }

        /// <summary>
        /// Руль заблокирован
        /// </summary>
        public int LockedSteeringValue { get; set; }

        /// <summary>
        /// Количество заблокированных колес
        /// </summary>
        public int LockedWheelsValue { get; set; }

        /// <summary>
        /// ТС перевернуто
        /// </summary>
        public int OverturnedValue { get; set; }

        /// <summary>
        /// ТС в кювете
        /// </summary>
        public int DitchValue { get; set; }

        public object Clone()
        {
            return new BasketAM
            {
                Distance = Distance,
                LoadingValue = LoadingValue,
                LockedSteeringValue = LockedSteeringValue,
                LockedWheelsValue = LockedWheelsValue,
                OverturnedValue = OverturnedValue,
                DitchValue = DitchValue
            };
        }
    }
}