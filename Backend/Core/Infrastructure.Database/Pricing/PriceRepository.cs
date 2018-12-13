using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Pricing
{
    public class PriceRepository : Repository<Price>, IPriceRepository
    {
        public PriceRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<Price> Get(int pricelistId, int catalogItemId)
        {
            return Entities.SingleOrDefaultAsync(i => i.PricelistId.Equals(pricelistId) 
                && i.CatalogItemId.Equals(catalogItemId));
        }
    }
}