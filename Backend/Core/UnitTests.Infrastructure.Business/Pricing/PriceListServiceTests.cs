using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;
using TransportSystems.Backend.Core.Infrastructure.Business.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Pricing
{
    public class PricelistServiceTestSuite
    {
        public PricelistServiceTestSuite()
        {
            PricelistRepositoryMock = new Mock<IPricelistRepository>();

            PricelistService = new PricelistService(PricelistRepositoryMock.Object);
        }

        public Mock<IPricelistRepository> PricelistRepositoryMock { get; }

        public IPricelistService PricelistService { get; }
    }

    public class PricelistServiceTests
    {
        public PricelistServiceTests()
        {
            Suite = new PricelistServiceTestSuite();
        }

        public PricelistServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreatePricelist()
        {
            var result = await Suite.PricelistService.Create();

            Suite.PricelistRepositoryMock
                .Verify(m => m.Add(It.IsAny<Pricelist>()));
            Suite.PricelistRepositoryMock
                .Verify(m => m.Save());

            Assert.NotNull(result);
        }
    }
}