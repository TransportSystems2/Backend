using Common.Models.Units;
using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Pricing;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Organization
{
    public class ApplicationGarageServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationGarageServiceTestSuite()
        {
            DomainGarageServiceMock = new Mock<IGarageService>();
            AddressServiceMock = new Mock<IApplicationAddressService>();
            PricelistServiceMock = new Mock<IApplicationPricelistService>();

            GarageService = new ApplicationGarageService(
                TransactionServiceMock.Object,
                DomainGarageServiceMock.Object,
                AddressServiceMock.Object,
                PricelistServiceMock.Object);
        }

        public IApplicationGarageService GarageService { get; }

        public Mock<IGarageService> DomainGarageServiceMock { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }

        public Mock<IApplicationPricelistService> PricelistServiceMock { get; }
    }

    public class ApplicationGarageServiceTests : BaseServiceTests<ApplicationGarageServiceTestSuite>
    {
        [Fact]
        public async Task CreateDomainGarage()
        {
            var commonId = 1;
            var pricelistId = commonId++;
            var isPublic = true;

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

            var domainCompany = new Company
            {
                Id = commonId++,
                Name = "Транспортные системы"
            };

            var domainPricelist = new Pricelist { Id = commonId++ };

            Suite.AddressServiceMock
                .Setup(m => m.CreateDomainAddress(AddressKind.Garage, address))
                .ReturnsAsync(domainAddress);
            Suite.PricelistServiceMock
                .Setup(m => m.CreateDomainPricelist(It.IsAny<PricelistAM>()))
                .ReturnsAsync(domainPricelist);

            await Suite.GarageService.CreateDomainGarage(domainCompany.Id, address);

            Suite.DomainGarageServiceMock
                .Verify(m => m.Create(domainCompany.Id,
                    domainAddress.Id));
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
        public async Task GetDomainGarageByAddress()
        {
            var address = new AddressAM
            {
                Country = "Россия",
                Province = "Ярославская",
                Locality = "Рыбинск",
                District = "Центральный"
            };

            await Suite.GarageService.GetDomainGarageByAddress(address);

            Suite.DomainGarageServiceMock
                .Verify(m => m.GetByAddress(
                    address.Country,
                    address.Province,
                    address.Locality,
                    address.District));
        }

        [Fact]
        public async Task GetDomainGarageByCoordinate()
        {
            var coordinate = new Coordinate
            {
                Latitude = 11.000,
                Longitude = 22.000
            };

            await Suite.GarageService.GetDomainGarageByCoordinate(coordinate);

            Suite.DomainGarageServiceMock
                .Verify(m => m.GetByCoordinate(
                    coordinate.Latitude,
                    coordinate.Longitude));
        }
    }
}