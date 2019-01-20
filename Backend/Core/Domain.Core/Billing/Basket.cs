using DotNetDistance;

namespace TransportSystems.Backend.Core.Domain.Core.Billing
{
    public class Basket : BaseEntity
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
    }
}