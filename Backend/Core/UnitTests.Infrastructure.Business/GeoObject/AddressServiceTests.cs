using Moq;

using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Domain.Interfaces.Geo;
using TransportSystems.Backend.Core.Infrastructure.Business.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;

using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Geo
{
    public class AddressServiceTestSuite
    {
        public AddressServiceTestSuite()
        {
            AddressRepositoryMock = new Mock<IAddressRepository>();

            AddressService = new AddressService(AddressRepositoryMock.Object);
        }

        public Mock<IAddressRepository> AddressRepositoryMock { get; }

        public IAddressService AddressService { get; }
    }

    public class AddressServiceTests
    {
        [Fact]
        public async void CreateAddressResultModel()
        {
            var suite = new AddressServiceTestSuite();

            var latitude = 12.2342342;
            var longitude = 45.2343243;
            var adjustedLatitude = 12.2342333;
            var adjustedLongitude = 45.2343256;

            var address = await suite.AddressService.Create(
                AddressKind.Other,
                "Ярославская область, г.Рыбинск, проспект Ленина, 176",
                "Россия",
                "Ярославская область",
                "Рыбинский район",
                "Рыбинск",
                null,
                "проспект Ленина",
                "176",
                latitude,
                longitude,
                adjustedLatitude,
                adjustedLongitude);

            suite.AddressRepositoryMock
                .Verify(m => 
                    m.Add(It.Is<Address>(a => 
                        a.Request.Equals(address.Request)
                        && a.Country.Equals(address.Country)
                        && a.Province.Equals(address.Province)
                        && a.Area.Equals(address.Area)
                        && a.Locality.Equals(address.Locality)
                        && (a.District == address.District)
                        && a.Street.Equals(address.Street)
                        && a.House.Equals(address.House)
                        && a.Latitude.Equals(latitude)
                        && a.Longitude.Equals(longitude)
                        && a.AdjustedLatitude.Equals(adjustedLatitude)
                        && a.AdjustedLongitude.Equals(adjustedLongitude))),
                    Times.Once);

            Assert.Equal("Ярославская область, г.Рыбинск, проспект Ленина, 176", address.Request);
            Assert.Equal("Россия", address.Country);
            Assert.Equal("Ярославская область", address.Province);
            Assert.Equal("Рыбинский район", address.Area);
            Assert.Equal("Рыбинск", address.Locality);
            Assert.Null(address.District);
            Assert.Equal("проспект Ленина", address.Street);
            Assert.Equal("176", address.House);

            Assert.Equal(latitude, address.Latitude);
            Assert.Equal(longitude, address.Longitude);
            Assert.Equal(adjustedLatitude, address.AdjustedLatitude);
            Assert.Equal(adjustedLongitude, address.AdjustedLongitude);
        }

        [Fact]
        public async Task GetShortNameResultShortName()
        {
            var suite = new AddressServiceTestSuite();
            var address = new Address
            {
                Id = 1,
                Locality = "Санкт-Петербург"
            };

            suite.AddressRepositoryMock
                .Setup(m => m.Get(address.Id))
                .ReturnsAsync(address);

            var shortName = await suite.AddressService.GetShortTitle(address.Id);

            Assert.Equal(address.Locality, shortName);
        }

        [Fact]
        public async Task GetProvincesWhereAddressKindIsGarage()
        {
            var suite = new AddressServiceTestSuite();
            var country = "Россия";
            var addressKind = AddressKind.Garage;
            
            var result = await suite.AddressService.GetProvinces(addressKind, country);

            suite.AddressRepositoryMock
                 .Verify(m => m.GetProvinces(addressKind, country, OrderingKind.None), Times.Once);
        }

        [Fact]
        public async Task GetLocalitiesWhereAddressKindIsGarage()
        {
            var suite = new AddressServiceTestSuite();
            var country = "Россия";
            var province = "Ярославская";
            var addressKind = AddressKind.Garage;

            var result = await suite.AddressService.GetLocalities(addressKind, country, province);

            suite.AddressRepositoryMock
                 .Verify(m => m.GetLocalities(addressKind, country, province, OrderingKind.None), Times.Once);
        }

        [Fact]
        public async Task GetDistrictsWhereAddressKindIsGarage()
        {
            var suite = new AddressServiceTestSuite();
            var country = "Россия";
            var province = "Ярославская";
            var locality = "Ярославль";
            var addressKind = AddressKind.Garage;

            var result = await suite.AddressService.GetDistricts(addressKind, country, province, locality);

            suite.AddressRepositoryMock
                 .Verify(m => m.GetDistricts(addressKind, country, province, locality, OrderingKind.None), Times.Once);
        }

        [Fact]
        public async Task GetProvincesWasOrderedByAscWhereKindIsGarage()
        {
            var suite = new AddressServiceTestSuite();
            var country = "Россия";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Asc;

            var result = await suite.AddressService.GetProvinces(addressKind, country, orderingKind);

            suite.AddressRepositoryMock
                 .Verify(m => m.GetProvinces(addressKind, country, orderingKind), Times.Once);
        }

        [Fact]
        public async Task GetProvincesWasOrderedByDesc()
        {
            var suite = new AddressServiceTestSuite();
            var country = "Россия";
            var addressKind = AddressKind.Garage;
            var orderingKind = OrderingKind.Desc;

            var result = await suite.AddressService.GetProvinces(addressKind, country, orderingKind);

            suite.AddressRepositoryMock
                 .Verify(m => m.GetProvinces(addressKind, country, orderingKind), Times.Once);
        }

        [Fact]
        public async Task GetAddressByGeocodingWhenSearchDepthToDistrict()
        {
            var suite = new AddressServiceTestSuite();
            var country = "Россия";
            var province = "Ярославская";
            var locality = "Рыбинск";
            var district = "Центральный";

            var result = await suite.AddressService.GetByGeocoding(AddressKind.Garage, country, province, locality, district);

            suite.AddressRepositoryMock
                .Verify(m => m.GetByGeocoding(AddressKind.Garage, country, province, locality, district, null, null), Times.Once);
        }

        [Fact]
        public async Task GetAddressesInCoordinateBounds()
        {
            var suite = new AddressServiceTestSuite();

            var kind = AddressKind.Garage;
            var minLatitude = 1;
            var minLongitude = 2;
            var maxLatitude = 3;
            var maxLongitude = 4;

            var result = await suite.AddressService.GetByCoordinateBounds(kind, minLatitude, minLongitude, maxLatitude, maxLongitude);

            suite.AddressRepositoryMock
                .Verify(m => m.GetInCoordinateBounds(
                    kind,
                    minLatitude,
                    minLongitude,
                    maxLatitude,
                    maxLongitude));
        }

        [Fact]
        public async Task GetAddressByCoordinate()
        {
            var address = new Address
            {
                Latitude = 11.1111,
                Longitude = 22.2222
            };

            var suite = new AddressServiceTestSuite();

            suite.AddressRepositoryMock
                .Setup(m => m.GetByCoordinate(address.Latitude, address.Longitude))
                .ReturnsAsync(address);

            var result = await suite.AddressService.GetByCoordinate(address.Latitude, address.Longitude);

            Assert.Equal(address, result);
        }
    }
}