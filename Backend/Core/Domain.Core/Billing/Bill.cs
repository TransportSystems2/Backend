namespace TransportSystems.Backend.Core.Domain.Core.Billing
{
    public class Bill : BaseEntity
    {
        public const float DefaultDegreeOfDifficulty = 1f;

        public Bill()
        {
            DegreeOfDifficulty = DefaultDegreeOfDifficulty;
        }

        public int PriceId { get; set; }

        public int BasketId { get; set;  }

        /// <summary>
        /// Комиссия системы
        /// </summary>
        public byte CommissionPercentage { get; set; }

        /// <summary>
        /// Процент сложности
        /// </summary>
        public float DegreeOfDifficulty { get; set; }

        public decimal TotalCost { get; set; }
    }
}