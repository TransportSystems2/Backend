namespace TransportSystems.Backend.Core.Domain.Core.Pricing
{
    public class Price : BaseEntity
    {
        public const byte DefaultComissionPercentage = 10;
        public const decimal DefaultPerMeterPrice = 0.04m;
        public const decimal DefaultLoadingPrice = 900m;
        public const decimal DefaultLockedSteeringPrice = 300m;
        public const decimal DefaultLockedWheelPrice = 500m;
        public const decimal DefaultOverturnedPrice = 1500m;
        public const decimal DefaultDitchPrice = 1000m;

        public int PricelistId { get; set; }

        public int CatalogItemId { get; set; }

        public string Name { get; set; }

        public byte CommissionPercentage { get; set; }

        /// <summary>
        /// Транспортировка
        /// </summary>
        public decimal PerMeter { get; set; }

        /// <summary>
        /// Погрузка
        /// </summary>
        public decimal Loading { get; set; }

        /// <summary>
        /// Погрузка с блокированными колесами
        /// </summary>
        public decimal LockedSteering { get; set; }

        /// <summary>
        /// Погрузка с заблокированным рулевым колесом
        /// </summary>
        public decimal LockedWheel { get; set; }

        /// <summary>
        /// Погрузка перевернутого ТС
        /// </summary>
        public decimal Overturned { get; set; }

        /// <summary>
        /// Погрузка ТС находящегося в кювете
        /// </summary>
        public decimal Ditch { get; set; }
    }
}