using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.API.Controllers.Cargo;
using TransportSystems.Backend.Application.Interfaces;
using Xunit;

namespace TransportSystems.Backend.API.UnitTests.API.Controllers.Cargo
{
    public class CargoControllerTestSuite
    {
        public CargoControllerTestSuite()
        {
            ApplicationCargoServiceMock = new Mock<IApplicationCargoService>();

            CargoController = new CargosController(ApplicationCargoServiceMock.Object);
        }

        public CargosController CargoController { get; }

        public Mock<IApplicationCargoService> ApplicationCargoServiceMock { get; }
    }

    public class CargoControllerTests
    {
        public CargoControllerTests()
        {
            Suite = new CargoControllerTestSuite();
        }

        protected CargoControllerTestSuite Suite { get; }

        [Fact]
        public async Task GetCatalogItems()
        {
            var result = await Suite.CargoController.GetCatalogItems();

            Suite.ApplicationCargoServiceMock.Verify(m => m.GetCatalogItems(), Times.Once);
        }

        [Fact]
        public async Task ValidRegistrationNumber()
        {
            var registrationNumber = "х827мн76";
            var result = await Suite.CargoController.ValidRegistrationNumber(registrationNumber);

            Suite.ApplicationCargoServiceMock.Verify(m => m.ValidRegistrationNumber(registrationNumber), Times.Once);
        }
    }
}
