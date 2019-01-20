using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;

namespace TransportSystems.Backend.Core.Services.Interfaces.Pricing
{
    public interface IPriceService : IDomainService<Price>
    {
        Task<Price> Create(
            int pricelistId,
            int catalogItemId,
            string name,
            byte commissionPercentage,
            decimal perMeter,
            decimal loading,
            decimal lockedSteering,
            decimal lockedWheel,
            decimal overturned,
            decimal ditch);

        Task<Price> Get(int pricelistId, int catalogItemId);
    }
}