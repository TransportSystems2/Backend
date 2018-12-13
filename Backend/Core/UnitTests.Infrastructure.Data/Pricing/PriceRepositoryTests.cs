using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Pricing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Data.Pricing
{ 
    public class PriceRepositoryTests : BaseRepositoryTests<IPriceRepository, Price>
    {
        [Fact]
        public async Task GetPrice()
        {
            var items = new[]
            {
                new Price { Id = 1, PricelistId = 1, CatalogItemId = 1 },
                new Price { Id = 3, PricelistId = 1, CatalogItemId = 2 },
                new Price { Id = 4, PricelistId = 2, CatalogItemId = 3 },
            };

            await Repository.AddRange(items);
            await Repository.Save();

            var priceItem = await Repository.Get(1, 2);

            Assert.Equal(3, priceItem.Id);
        }

        protected override IPriceRepository CreateRepository(ApplicationContext context)
        {
            return new PriceRepository(context);
        }
    }
}
