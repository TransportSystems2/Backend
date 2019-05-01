using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Infrastructure.Business.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Oraganization
{
    public class GarageServiceTestSuite
    {
        public GarageServiceTestSuite()
        {
            RepositoryMock = new Mock<IGarageRepository>();
            AddressServiceMock = new Mock<IAddressService>();
            PricelistServiceMock = new Mock<IPricelistService>();
            CompanyServiceMock = new Mock<ICompanyService>();

            Service = new GarageService(RepositoryMock.Object,
                CompanyServiceMock.Object,
                AddressServiceMock.Object,
                PricelistServiceMock.Object);
        }
        public IGarageService Service { get; }

        public Mock<IGarageRepository> RepositoryMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<IAddressService> AddressServiceMock { get; }

        public Mock<IPricelistService> PricelistServiceMock { get; }
    }

    public class GarageServiceTests
    {
        public GarageServiceTests()
        {
            Suite = new GarageServiceTestSuite();
        }

        public GarageServiceTestSuite Suite { get; }

        [Fact]
        public async void CreateGarage()
        {
            var commonId = 1;

            var isPublic = true;
            var companyId = commonId++;
            var cityId = commonId++;
            var addressId = commonId++;
            var pricelistId = commonId++;

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);

            Suite.AddressServiceMock
                .Setup(m => m.IsExist(addressId))
                .ReturnsAsync(true);

            Suite.PricelistServiceMock
                .Setup(m => m.IsExist(pricelistId))
                .ReturnsAsync(true);
        
            var result = await Suite.Service.Create(isPublic, companyId, addressId, pricelistId);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<Garage>(
                    g => g.IsPublic.Equals(isPublic)
                    && g.CompanyId.Equals(companyId)
                    && g.AddressId.Equals(addressId)
                    && g.PricelistId.Equals(pricelistId))));
            Suite.RepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(isPublic, result.IsPublic);
            Assert.Equal(companyId, result.CompanyId);
            Assert.Equal(addressId, result.AddressId);
            Assert.Equal(pricelistId, result.PricelistId);
        }

        [Fact]
        public async Task GetAvailableProvince()
        {
            var country = "Россия";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Asc;

            var result = await Suite.Service.GetAvailableProvinces(country);

            Suite.AddressServiceMock
                 .Verify(m => m.GetProvinces(addressKind, country, orderingKind), Times.Once);
        }

        [Fact]
        public async Task GetAvailableLocalities()
        {
            var country = "Россия";
            var province = "Ярославская область";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Asc;

            var result = await Suite.Service.GetAvailableLocalities(country, province);

            Suite.AddressServiceMock
                 .Verify(m => m.GetLocalities(addressKind, country, province, orderingKind), Times.Once);
        }

        [Fact]
        public async Task GetAvailableDistricts()
        {
            var country = "Россия";
            var province = "Ярославская";
            var locality = "Ярославль";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Asc;

            var result = await Suite.Service.GetAvailableDistricts(country, province, locality);

            Suite.AddressServiceMock
                 .Verify(m => m.GetDistricts(addressKind, country, province, locality, orderingKind), Times.Once);
        }

        [Fact]
        public async Task GetByCoordinate()
        {
            var commonId = 1;

            var address = new Address
            {
                Id = commonId++,
                Latitude = 11.0000,
                Longitude = 22.0000
            };

            var garage = new Garage
            {
                Id = commonId,
                AddressId = address.Id
            };

            Suite.AddressServiceMock
                .Setup(m => m.GetByCoordinate(address.Latitude, address.Longitude))
                .ReturnsAsync(address);
            Suite.RepositoryMock
                .Setup(m => m.GetByAddress(address.Id))
                .ReturnsAsync(garage);

            var result = await Suite.Service.GetByCoordinate(address.Latitude, address.Longitude);

            Assert.Equal(garage, result);
            Assert.Equal(garage.Id, result.Id);
            Assert.Equal(garage.AddressId, result.AddressId);
        }
    }
}