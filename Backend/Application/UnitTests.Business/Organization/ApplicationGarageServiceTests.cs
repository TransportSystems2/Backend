using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Business.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Application.UnitTests.Business.Organization
{
    public class ApplicationGarageServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationGarageServiceTestSuite()
        {
            DomainGarageServiceMock = new Mock<IGarageService>();
            CityServiceMock = new Mock<IApplicationCityService>();
            AddressServiceMock = new Mock<IApplicationAddressService>();

            GarageService = new ApplicationGarageService(
                TransactionServiceMock.Object,
                DomainGarageServiceMock.Object,
                CityServiceMock.Object,
                AddressServiceMock.Object);
        }

        public IApplicationGarageService GarageService { get; }

        public Mock<IGarageService> DomainGarageServiceMock { get; }

        public Mock<IApplicationCityService> CityServiceMock { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }
    }

    public class ApplicationGarageServiceTests : BaseServiceTests<ApplicationGarageServiceTestSuite>
    {
        [Fact]
        public async Task CreateDomainGarage()
        {
            var commonId = 1;
            var cityId = commonId++;
            var pricelistId = commonId++;

            var address = new AddressAM
            {
                Country = "Россия",
                Province = "Московская область",
                Area = "Москва",
                Locality = "Москва",
                District = "Северо-Восточный район",
                Latitude = 55.771899,
                Longitude = 37.597576,
            };

            var domainAddress = new Address
            {
                Id = commonId++,
                Country = "Россия",
                Province = "Московская область",
                Area = "Москва",
                Locality = "Москва",
                District = "Северо-Восточный район",
                Latitude = 55.771899,
                Longitude = 37.597576,
            };

            var domainCity = new City
            {
                Id = cityId,
                PricelistId = pricelistId
            };

            Suite.CityServiceMock
                .Setup(m => m.GetDomainCity(cityId))
                .ReturnsAsync(domainCity);
            Suite.AddressServiceMock
                .Setup(m => m.CreateDomainAddress(AddressKind.Garage, address))
                .ReturnsAsync(domainAddress);

            await Suite.GarageService.CreateDomainGarage(cityId, address);

            Suite.DomainGarageServiceMock
                .Verify(m => m.Create(cityId, domainAddress.Id, domainCity.PricelistId));
        }

        [Fact]
        public async Task GetDomainGarage()
        {
            var garageId = 1;

            await Suite.GarageService.GetDomainGarage(garageId);

            Suite.DomainGarageServiceMock
                .Verify(m => m.Get(garageId));
        }

        [Fact]
        public async Task GetAvailableProvince()
        {
            var country = "Россия";

            var result = await Suite.GarageService.GetAvailableProvinces(country);

            Suite.DomainGarageServiceMock
                .Verify(m => m.GetAvailableProvinces(country), Times.Once);
        }

        [Fact]
        public async Task GetAvailableLocalities()
        {
            var country = "Россия";
            var province = "Ярославская область";

            var result = await Suite.GarageService.GetAvailableLocalities(country, province);

            Suite.DomainGarageServiceMock
                .Verify(m => m.GetAvailableLocalities(country, province), Times.Once);
        }

        [Fact]
        public async Task GetAvailableDistricts()
        {
            var country = "Россия";
            var province = "Ярославская";
            var locality = "Ярославль";

            var result = await Suite.GarageService.GetAvailableDistricts(country, province, locality);

            Suite.DomainGarageServiceMock
                .Verify(m => m.GetAvailableDistricts(country, province, locality), Times.Once);
        }
    }
}