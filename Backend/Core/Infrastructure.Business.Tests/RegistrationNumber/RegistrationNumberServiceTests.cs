using System.Threading.Tasks;
using TransportSystems.Backend.Core.Infrastructure.Business.RegistrationNumber;
using TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.RegistrationNumber
{
    public class RegistrationNumberServiceTestSuite
    {
        public RegistrationNumberServiceTestSuite()
        {
            RegistrationService = new RegistrationNumberService();
        }

        public IRegistrationNumberService RegistrationService { get; }
    }

    public class RegistrationNumberServiceTests
    {
        public RegistrationNumberServiceTests()
        {
            Suite = new RegistrationNumberServiceTestSuite();
        }

        protected RegistrationNumberServiceTestSuite Suite { get; }

        [Fact]
        public async Task ValidRegistrationNumber()
        {
            var valid = await Suite.RegistrationService.ValidRegistrationNumber("Х827МН76");

            Assert.True(valid);
        }

        [Fact]
        public async Task NotValidRegistrationNumber()
        {
            var valid = await Suite.RegistrationService.ValidRegistrationNumber("х827мн76");

            Assert.False(valid);
        }

        [Fact]
        public async Task ValidEmptyRegistrationNumber()
        {
            var valid = await Suite.RegistrationService.ValidRegistrationNumber(string.Empty);

            Assert.False(valid);
        }
    }
}
