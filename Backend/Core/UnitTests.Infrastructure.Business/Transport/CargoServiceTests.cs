using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;
using TransportSystems.Backend.Core.Infrastructure.Business.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Transport
{
    public class CargoServiceTestSuite
    {
        public CargoServiceTestSuite()
        {
            CargoRepositoryMock = new Mock<ICargoRepository>();

            CargoService = new CargoService(
                CargoRepositoryMock.Object);
        }

        public Mock<ICargoRepository> CargoRepositoryMock { get; }

        public ICargoService CargoService { get; }
    }

    public class CargoServiceTests
    {
        public CargoServiceTests()
        {
            Suite = new CargoServiceTestSuite();
        }

        private CargoServiceTestSuite Suite { get; }

        [Fact]
        public async Task Create()
        {
            var commonId = 1;
            var weightCatalogItemId = commonId++;
            var kindCatalogItemId = commonId++;
            var brandCatalogItemId = commonId++;
            var registrationNumber = "а123аа78";
            var comment = "Тачка президента";

            var result = await Suite.CargoService.Create(
                weightCatalogItemId,
                kindCatalogItemId,
                brandCatalogItemId,
                registrationNumber,
                comment);

            Suite.CargoRepositoryMock
                .Verify(m => m.Add(result));
            Suite.CargoRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(weightCatalogItemId, result.WeightCatalogItemId);
            Assert.Equal(kindCatalogItemId, result.KindCatalogItemId);
            Assert.Equal(brandCatalogItemId, result.BrandCatalogItemId);
            Assert.Equal(registrationNumber, result.RegistrationNumber);
            Assert.Equal(comment, result.Comment);
        }
    }
}