using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;

namespace TransportSystems.Backend.Core.Services.Interfaces.Billing
{
    public interface IBillService : IDomainService<Bill>
    {
        Task<Bill> Create(
            int priceId,
            int basketId,
            byte commissionPercentage,
            float degreeOfDifficulty);

        Task SetTotalCost(int billId, decimal totalCost);
    }
}