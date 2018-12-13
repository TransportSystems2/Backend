using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Organization;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Data.Organization
{
    public class CityRepositoryTests : BaseRepositoryTests<ICityRepository, City>
    {
        [Fact]
        public async Task GetCityByDomain()
        {
            var entities = new []
            {
                new City { Id = 1, Domain = "moscow" },
                new City { Id = 2, Domain = "rybinsk" }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByDomain("rybinsk");

            Assert.Equal(entities[1], result);
        }

        [Fact]
        public async Task GetCityByAddress()
        {
            var entities = new[]
            {
                new City { Id = 1, AddressId = 3, },
                new City { Id = 2, AddressId = 4 }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByAddress(4);

            Assert.Equal(entities[1], result);
        }

        [Fact]
        public async Task IsExistByDomain()
        {
            var entities = new[]
{
                new City { Id = 1, Domain = "moscow" },
                new City { Id = 2, Domain = "rybinsk" }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.IsExistByDomain("rybinsk");

            Assert.True(result);
        }

        [Fact]
        public async Task IsExistByDomainWhereDomainDoesNotExist()
        {
            var entities = new[]
{
                new City { Id = 1, Domain = "moscow" },
                new City { Id = 2, Domain = "rybinsk" }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.IsExistByDomain("yaroslavl");

            Assert.False(result);
        }

        protected override ICityRepository CreateRepository(ApplicationContext context)
        {
            return new CityRepository(context);
        }
    }
}