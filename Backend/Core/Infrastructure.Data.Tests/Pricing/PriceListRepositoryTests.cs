using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Pricing;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests.Pricing
{
    public class PricelistRepositoryTests : BaseRepositoryTests<IPricelistRepository, Pricelist>
    {
        protected override IPricelistRepository CreateRepository(ApplicationContext context)
        {
            return new PricelistRepository(context);
        }
    }
}
