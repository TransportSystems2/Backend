namespace TransportSystems.Backend.Application.Models.Pricing
{
    public class PriceAM : BaseAM
    {
        public string Name { get; set; }

        public int CatalogItemId { get; set; }

        public byte CommissionPercentage { get; set; }

        /// <summary>
        /// Транспортировка (за метр)
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