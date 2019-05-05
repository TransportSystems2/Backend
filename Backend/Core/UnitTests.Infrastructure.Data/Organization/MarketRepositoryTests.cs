using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Organization;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Data.Organization
{
    public class MarketRepositoryTests : BaseRepositoryTests<IMarketRepository, Market>
    {
        [Fact]
        public async Task GetByAddress()
        {
            var items = new[]
            {
                new Market { Id = 1, AddressId = 1 },
                new Market { Id = 2, AddressId = 2 },
                new Market { Id = 3, AddressId = 3 },
                new Market { Id = 4, AddressId = 4 },
            };

            await Repository.AddRange(items);
            await Repository.Save();

            var market = await Repository.GetByAddress(3);

            Assert.Equal(3, market.Id);
        }

        [Fact]
        public async Task GetByAddressIds()
        {
            var items = new[]
            {
                new Market { Id = 1, AddressId = 1 },
                new Market { Id = 2, AddressId = 2 },
                new Market { Id = 3, AddressId = 3 },
                new Market { Id = 4, AddressId = 4 },
            };

            await Repository.AddRange(items);
            await Repository.Save();

            var markets = await Repository.GetByAddressIds(new List<int> { 1, 3, 4 });

            Assert.Equal(3, markets.Count);
            Assert.Equal(1, markets.ElementAt(0).Id);
            Assert.Equal(3, markets.ElementAt(1).Id);
            Assert.Equal(4, markets.ElementAt(2).Id);
        }

        protected override IMarketRepository CreateRepository(ApplicationContext context)
        {
            return new MarketRepository(context);
        }
    }
}