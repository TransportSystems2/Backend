using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Pricing
{
    public class PricelistRepository : Repository<Pricelist>, IPricelistRepository
    {
        public PricelistRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}