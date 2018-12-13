using TransportSystems.Backend.Core.Domain.Core.Billing;

namespace TransportSystems.Backend.Application.Models.Billing
{
    public class BillInfoAM : BaseAM
    {
        public BillInfoAM()
        {
            DegreeOfDifficulty = Bill.DefaultDegreeOfDifficulty;
        }

        public int PriceId { get; set; }

        /// <summary>
        /// Комиссия системы
        /// </summary>
        public byte CommissionPercentage { get; set; }

        /// <summary>
        /// Процент сложности
        /// </summary>
        public float DegreeOfDifficulty { get; set; }
    }
}