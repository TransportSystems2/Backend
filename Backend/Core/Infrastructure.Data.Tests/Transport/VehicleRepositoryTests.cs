using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;
using TransportSystems.Backend.Core.Infrastructure.Database;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests.Transport
{
    public class VehicleRepositoryTests : BaseRepositoryTests<IVehicleRepository, Vehicle>
    {
        [Fact]
        public async Task GetByCompany_Result_NumberOfVehicleEqualsNumberFromDB()
        {
            var entities = new[]
            {
                new Vehicle { Id = 1, CompanyId = 4 },
                new Vehicle { Id = 2, CompanyId = 4 },
                new Vehicle { Id = 3, CompanyId = 5}
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByCompany(4);

            Assert.Equal(2, result.Count);
        }

        protected override IVehicleRepository CreateRepository(ApplicationContext context)
        {
            return new VehicleRepository(context);
        }
    }
}