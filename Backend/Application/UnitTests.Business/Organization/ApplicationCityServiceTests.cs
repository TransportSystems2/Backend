using Common.Models.Geolocation;
using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Business.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Pricing;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Organization
{
    public class ApplicationCityServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationCityServiceTestSuite()
        {
            DomainCityServiceMock = new Mock<ICityService>();
            AddressServiceMock = new Mock<IApplicationAddressService>();
            PricelistServiceMock = new Mock<IApplicationPricelistService>();

            CityService = new ApplicationCityService(
                TransactionServiceMock.Object,
                DomainCityServiceMock.Object,
                AddressServiceMock.Object,
                PricelistServiceMock.Object);
        }

        public IApplicationCityService CityService { get; }

        public Mock<ICityService> DomainCityServiceMock { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }

        public Mock<IApplicationPricelistService> PricelistServiceMock { get; }
    }

    public class ApplicationCityServiceTests : BaseServiceTests<ApplicationCityServiceTestSuite>
    {
        [Fact]
        public async Task CreateCity()
        {
            var commonId = 1;

            var address = new AddressAM
            {
                Country = "Россия",
                Province = "Ярославская область",
                Area = "Пошехонский район",
                Locality = "Пошехонье",
                Latitude = 11.1111,
                Longitude = 22.2222
            };

            var domainAddress = new Address { Id = commonId++ };
            var domainPricelist = new Pricelist { Id = commonId++ };

            var domain = "poshehon";

            Suite.AddressServiceMock
                .Setup(m => m.CreateDomainAddress(AddressKind.City, address))
                .ReturnsAsync(domainAddress);
            Suite.PricelistServiceMock
                .Setup(m => m.CreateDomainPricelist())
                .ReturnsAsync(domainPricelist);

            var result = await Suite.CityService.CreateDomainCity(domain, address);

            Suite.DomainCityServiceMock
                .Verify(m => m.Create(domain, domainAddress.Id, domainPricelist.Id));
        }

        [Fact]
        public async Task GetDomainCityByCoordinate()
        {
            var commonId = 1;

            var coordinate = new Coordinate
            {
                Latitude = 11.1111,
                Longitude = 22.2222
            };

            var domainAddress = new Address
            {
                Id = commonId++,
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude
            };

            var domainCity = new City();

            Suite.AddressServiceMock
                .Setup(m => m.GetDomainAddressByCoordinate(coordinate))
                .ReturnsAsync(domainAddress);
            Suite.DomainCityServiceMock
                .Setup(m => m.GetByAddress(domainAddress.Id))
                .ReturnsAsync(domainCity);

            var result = await Suite.CityService.GetDomainCityByCoordinate(coordinate);

            Assert.Equal(domainCity, result);
        }

        [Fact]
        public async Task IsExistByDomain()
        {
            var domain = "moscow";
            var isExist = true;

            Suite.DomainCityServiceMock
                .Setup(m => m.IsExistByDomain(domain))
                .ReturnsAsync(isExist);

            var result = await Suite.CityService.IsExistByDomain(domain);

            Assert.True(result);
        }

        [Fact]
        public async Task GetDomainCity()
        {
            var cityId = 1;
            var domainCity = new City { Id = cityId };

            Suite.DomainCityServiceMock
                .Setup(m => m.Get(cityId))
                .ReturnsAsync(domainCity);

            var result = await Suite.CityService.GetDomainCity(cityId);

            Assert.Equal(domainCity, result);
        }
    }
}