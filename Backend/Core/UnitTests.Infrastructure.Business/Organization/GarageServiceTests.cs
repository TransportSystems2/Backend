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
            GarageRepositoryMock = new Mock<IGarageRepository>();
            CityServiceMock = new Mock<ICityService>();
            AddressServiceMock = new Mock<IAddressService>();
            PricelistServiceMock = new Mock<IPricelistService>();
            CompanyServiceMock = new Mock<ICompanyService>();

            GarageService = new GarageService(GarageRepositoryMock.Object,
                CompanyServiceMock.Object,
                CityServiceMock.Object,
                AddressServiceMock.Object,
                PricelistServiceMock.Object);
        }
        public IGarageService GarageService { get; }

        public Mock<IGarageRepository> GarageRepositoryMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<ICityService> CityServiceMock { get; }
        
        public Mock<IAddressService> AddressServiceMock { get; }

        public Mock<IPricelistService> PricelistServiceMock { get; }
    }

    public class GarageServiceTests
    {
        [Fact]
        public async void CreateGarage()
        {
            var commonId = 1;
            var suite = new GarageServiceTestSuite();

            var companyId = commonId++;
            var cityId = commonId++;
            var addressId = commonId++;
            var pricelistId = commonId++;

            suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);

            suite.CityServiceMock
                .Setup(m => m.IsExist(cityId))
                .ReturnsAsync(true);

            suite.AddressServiceMock
                .Setup(m => m.IsExist(addressId))
                .ReturnsAsync(true);

            suite.PricelistServiceMock
                .Setup(m => m.IsExist(pricelistId))
                .ReturnsAsync(true);
        
            var result = await suite.GarageService.Create(companyId, cityId, addressId, pricelistId);

            suite.GarageRepositoryMock
                .Verify(m => m.Add(It.Is<Garage>(
                    g => g.CityId.Equals(cityId)
                    && g.CompanyId.Equals(companyId)
                    && g.AddressId.Equals(addressId)
                    && g.PricelistId.Equals(pricelistId))));
            suite.GarageRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(companyId, result.CompanyId);
            Assert.Equal(cityId, result.CityId);
            Assert.Equal(addressId, result.AddressId);
            Assert.Equal(pricelistId, result.PricelistId);
        }

        [Fact]
        public async Task GetAvailableProvince()
        {
            var suite = new GarageServiceTestSuite();
            var country = "Россия";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Asc;

            var result = await suite.GarageService.GetAvailableProvinces(country);

            suite.AddressServiceMock
                 .Verify(m => m.GetProvinces(addressKind, country, orderingKind), Times.Once);
        }

        [Fact]
        public async Task GetAvailableLocalities()
        {
            var suite = new GarageServiceTestSuite();
            var country = "Россия";
            var province = "Ярославская область";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Asc;

            var result = await suite.GarageService.GetAvailableLocalities(country, province);

            suite.AddressServiceMock
                 .Verify(m => m.GetLocalities(addressKind, country, province, orderingKind), Times.Once);
        }

        [Fact]
        public async Task GetAvailableDistricts()
        {
            var suite = new GarageServiceTestSuite();
            var country = "Россия";
            var province = "Ярославская";
            var locality = "Ярославль";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Asc;

            var result = await suite.GarageService.GetAvailableDistricts(country, province, locality);

            suite.AddressServiceMock
                 .Verify(m => m.GetDistricts(addressKind, country, province, locality, orderingKind), Times.Once);
        }
    }
}