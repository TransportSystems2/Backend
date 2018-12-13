using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Users;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Data.Users
{
    public class DriverRepositoryTests : BaseRepositoryTests<IDriverRepository, Driver>
    {
        [Fact]
        public async Task GetByCompany()
        {
            var entities = new[]
            {
                new Driver { Id = 1, CompanyId = 4 },
                new Driver { Id = 2, CompanyId = 5 },
                new Driver { Id = 3, CompanyId = 4 }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByCompany(4);

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.ElementAt(0).Id);
            Assert.Equal(3, result.ElementAt(1).Id);
        }

        protected override IDriverRepository CreateRepository(ApplicationContext context)
        {
            return new DriverRepository(context);
        }
    }
}