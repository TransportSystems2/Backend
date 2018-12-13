using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Pricing
{
    public interface IPriceRepository : IRepository<Price>
    {
        Task<Price> Get(int pricelistId, int catalogItemId);
    }
}