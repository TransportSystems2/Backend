using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.API.Controllers.Address;
using TransportSystems.Backend.Application.Interfaces.Geo;
using Xunit;

namespace TransportSystems.Backend.API.UnitTests.API.Controllers.Address
{
    public class AddressControllerTestSuite
    {
        public AddressControllerTestSuite()
        {
            ApplicationServiceMock = new Mock<IApplicationAddressService>();
            Controller = new AddressesController(ApplicationServiceMock.Object);
        }

        public AddressesController Controller { get; set; }

        public Mock<IApplicationAddressService> ApplicationServiceMock { get; }
    }

    public class AddressControllerTests
    {
        public AddressControllerTests()
        {
            Suite = new AddressControllerTestSuite();
        }

        protected AddressControllerTestSuite Suite { get; }

        [Fact]
        public async Task Geocode()
        {
            var request = "Москв";
            var addressess = await Suite.Controller.Geocode(request, 5);

            Suite.ApplicationServiceMock.Verify(m => m.Geocode(request, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ReverseGeocode()
        {
            var latitude = 59.915359;
            var longitude = 30.435630;

            var addressess = await Suite.Controller.ReverseGeocode(latitude, longitude);

            Suite.ApplicationServiceMock.Verify(m => m.ReverseGeocode(latitude, longitude), Times.Once);
        }
    }
}