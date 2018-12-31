using Common.Models;
using Common.Models.Geolocation;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.External.Interfaces.Services.Direction;
using TransportSystems.Backend.External.Interfaces.Services.Geocoder;
using TransportSystems.Backend.External.Interfaces.Services.Maps;
using TransportSystems.Backend.External.Models.Geo;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Geo
{
    public class ApplicationAddressServiceTestsSuite : TransactionTestsSuite
    {
        public ApplicationAddressServiceTestsSuite()
        {
            DomainAddressServiceMock = new Mock<IAddressService>();
            DirectionServiceMock = new Mock<IDirectionService>();
            GeocoderServiceMock = new Mock<IGeocoderService>();
            MapsServiceMock = new Mock<IMapsService>();

            AddressService = new ApplicationAddressService(
                TransactionServiceMock.Object,
                MappingService,
                DomainAddressServiceMock.Object,
                DirectionServiceMock.Object,
                GeocoderServiceMock.Object,
                MapsServiceMock.Object);
        }

        public IApplicationAddressService AddressService { get; }

        public Mock<IAddressService> DomainAddressServiceMock { get; }

        public Mock<IDirectionService> DirectionServiceMock { get; }

        public Mock<IGeocoderService> GeocoderServiceMock { get; }

        public Mock<IMapsService> MapsServiceMock { get; } 
    }

    public class ApplicationAddressServiceTests : BaseServiceTests<ApplicationAddressServiceTestsSuite>
    {
        [Fact]
        public async Task GetDomainAddressByApplicationAddressWhenDomainAddressDoesNotExist()
        {
            var ApplicationAddress = new AddressAM
            {
                Country = "Россия",
                Province = "Московская область",
                Area = "Москва",
                Locality = "Москва",
                District = "Северо-Восточный район",
                Street = "4-я Тверская-Ямская улица",
                House = "7",
                Latitude = 55.771899,
                Longitude = 37.597576,
                AdjustedLatitude = 55.771800,
                AdjustedLongitude = 37.597500,
                FormattedText = "Россия, Москва, 4-я Тверская-Ямская улица, 7",
                Request = "Москва 4-я Тверская улица 7"
            };

            Suite.DomainAddressServiceMock
                .Setup(m => m.GetByCoordinate(ApplicationAddress.Latitude, ApplicationAddress.Longitude))
                .Returns(Task.FromResult<Address>(null));

            await Suite.AddressService.GetOrCreateDomainAddress(ApplicationAddress);

            Suite.DomainAddressServiceMock
                .Verify(m => m.Create(
                    AddressKind.Other,
                    ApplicationAddress.Request,
                    ApplicationAddress.Country,
                    ApplicationAddress.Province,
                    ApplicationAddress.Area,
                    ApplicationAddress.Locality,
                    ApplicationAddress.District,
                    ApplicationAddress.Street,
                    ApplicationAddress.House,
                    ApplicationAddress.Latitude,
                    ApplicationAddress.Longitude,
                    ApplicationAddress.AdjustedLatitude,
                    ApplicationAddress.AdjustedLongitude));
        }

        [Fact]
        public async Task GetDomainAddressByApplicationAddressWhenDomainAddressExist()
        {
            var ApplicationAddress = new AddressAM
            {
                Latitude = 55.771899,
                Longitude = 37.597576
            };

            var domainAddress = new Address
            {
                Latitude = ApplicationAddress.Latitude,
                Longitude = ApplicationAddress.Longitude
            };

            Suite.DomainAddressServiceMock
                .Setup(m => m.GetByCoordinate(ApplicationAddress.Latitude, ApplicationAddress.Longitude))
                .ReturnsAsync(domainAddress);

            var result = await Suite.AddressService.GetOrCreateDomainAddress(ApplicationAddress);

            Suite.DomainAddressServiceMock
                .Verify(m => m.Create(
                    It.IsAny<AddressKind>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>())
                    , Times.Never);
        }

        [Fact]
        public async Task GetDomainAddress()
        {
            var addressId = 1;

            var domainAddress = new Address { Id = addressId };

            Suite.DomainAddressServiceMock
                .Setup(m => m.Get(addressId))
                .ReturnsAsync(domainAddress);

            var result = await Suite.AddressService.GetDomainAddress(addressId);

            Assert.Equal(domainAddress, result);
        }

        [Fact]
        public async Task GetAddress()
        {
            var addressId = 1;

            var domainAddress = new Address
            {
                Country = "Россия",
                Province = "Московская область",
                Area = "Москва",
                Locality = "Москва",
                District = "Северо-Восточный район",
                Street = "4-я Тверская-Ямская улица",
                House = "7",
                Latitude = 55.771899,
                Longitude = 37.597576,
                AdjustedLatitude = 55.771800,
                AdjustedLongitude = 37.597500,
                FormattedText = "Россия, Москва, 4-я Тверская-Ямская улица, 7",
                Request = "Москва 4-я Тверская улица 7"
            };

            Suite.DomainAddressServiceMock
                .Setup(m => m.Get(addressId))
                .ReturnsAsync(domainAddress);

            var result = await Suite.AddressService.GetAddress(addressId);

            Assert.Equal(domainAddress.Request, result.Request);
            Assert.Equal(domainAddress.Country, result.Country);
            Assert.Equal(domainAddress.Province, result.Province);
            Assert.Equal(domainAddress.Area, result.Area);
            Assert.Equal(domainAddress.Locality, result.Locality);
            Assert.Equal(domainAddress.District, result.District);
            Assert.Equal(domainAddress.Street, result.Street);
            Assert.Equal(domainAddress.House, result.House);
            Assert.Equal(domainAddress.Latitude, result.Latitude);
            Assert.Equal(domainAddress.Longitude, result.Longitude);
            Assert.Equal(domainAddress.AdjustedLatitude, result.AdjustedLatitude);
            Assert.Equal(domainAddress.AdjustedLongitude, result.AdjustedLongitude);
        }

        [Fact]
        public async Task GetShortTitle()
        {
            var addressId = 1;

            await Suite.AddressService.GetShortTitle(addressId);

            Suite.DomainAddressServiceMock
                .Verify(m => m.GetShortTitle(addressId));
        }

        [Fact]
        public async Task Geocode()
        {
            var externalAddresses = new List<AddressEM>
            {
                new AddressEM { Country = "Россия", Province = "Краснодарский край" },
                new AddressEM { Country = "Россия", Province = "Красноярский край"}
            };

            var request = "красно";
            var maxResultCount = 7;

            Suite.GeocoderServiceMock
                .Setup(m => m.Geocode(request, maxResultCount))
                .ReturnsAsync(externalAddresses);

            var result = await Suite.AddressService.Geocode(request, maxResultCount);

            Assert.Equal(externalAddresses.Count, result.Count);
        }

        [Fact]
        public async Task GetNearestDomainAddresses()
        {
            var addressKind = AddressKind.City;
            var coordinate = new Coordinate { Latitude = 1.3, Longitude = 2.3 };
            var maxResultCount = 3;
            var distance = 500;
            var coordinateBounds = new CoordinateBounds
            {
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude,
                MinLatitude = 1,
                MinLongitude = 2,
                MaxLatitude = 3,
                MaxLongitude = 4
            };

            var domainAddresses = new List<Address>
            {
                new Address { Id = 1, Latitude = 1.1, Longitude = 2.1 },
                new Address { Id = 2, Latitude = 1.2, Longitude = 2.2 },
                new Address { Id = 3, Latitude = 1.3, Longitude = 2.3 },
                new Address { Id = 4, Latitude = 1.4, Longitude = 2.4 },
                new Address { Id = 5, Latitude = 1.5, Longitude = 2.5 }
            };

            var coordinates = domainAddresses.Select(address => address.ToCoordinate()).ToList();
            var orderedCoordinates = coordinates.GetRange(1, 3);

            Suite.DirectionServiceMock
                .Setup(m => m.GetCoordinateBounds(coordinate, distance))
                .ReturnsAsync(coordinateBounds);
            Suite.DomainAddressServiceMock
                .Setup(m => m.GetByCoordinateBounds(
                    addressKind,
                    coordinateBounds.MinLatitude,
                    coordinateBounds.MinLongitude,
                    coordinateBounds.MaxLatitude,
                    coordinateBounds.MaxLongitude))
                .ReturnsAsync(domainAddresses);
            Suite.DirectionServiceMock
                .Setup(m => m.GetNearestCoordinates(coordinate, It.IsAny<IEnumerable<Coordinate>>(), maxResultCount))
                .ReturnsAsync(orderedCoordinates);

            var result = await Suite.AddressService.GetNearestAddresses(addressKind, coordinate, distance, maxResultCount);

            Assert.Equal(maxResultCount, result.Count);

            Assert.Equal(1.2, result.ElementAt(0).Latitude);
            Assert.Equal(2.2, result.ElementAt(0).Longitude);

            Assert.Equal(1.3, result.ElementAt(1).Latitude);
            Assert.Equal(2.3, result.ElementAt(1).Longitude);

            Assert.Equal(1.4, result.ElementAt(2).Latitude);
            Assert.Equal(2.4, result.ElementAt(2).Longitude);
        }

        [Fact]
        public async Task GetDomainAddressByCoordinate()
        {
            var coordinate = new Coordinate
            {
                Latitude = 11.1111,
                Longitude = 22.2222
            };

            var domainAddress = new Address();

            Suite.DomainAddressServiceMock
                .Setup(m => m.GetByCoordinate(coordinate.Latitude, coordinate.Longitude))
                .ReturnsAsync(domainAddress);

            var result = await Suite.AddressService.GetDomainAddressByCoordinate(coordinate);

            Assert.Equal(domainAddress, result);
        }

        [Fact]
        public void FromDomainAddress()
        {
            var domainAddress = new Address
            {
                Country = "Россия",
                Province = "Московская область",
                Area = "Москва",
                Locality = "Москва",
                District = "Северо-Восточный район",
                Street = "4-я Тверская-Ямская улица",
                House = "7",
                Latitude = 55.771899,
                Longitude = 37.597576,
                AdjustedLatitude = 55.771800,
                AdjustedLongitude = 37.597500,
                FormattedText = "Россия, Москва, 4-я Тверская-Ямская улица, 7",
                Request = "Москва 4-я Тверская улица 7"
            };

            var ApplicationAddress = Suite.AddressService.FromDomainAddress(domainAddress);

            Assert.Equal(domainAddress.Country, ApplicationAddress.Country);
            Assert.Equal(domainAddress.Province, ApplicationAddress.Province);
            Assert.Equal(domainAddress.Area, ApplicationAddress.Area);
            Assert.Equal(domainAddress.Locality, ApplicationAddress.Locality);
            Assert.Equal(domainAddress.District, ApplicationAddress.District);
            Assert.Equal(domainAddress.Street, ApplicationAddress.Street);
            Assert.Equal(domainAddress.House, ApplicationAddress.House);
            Assert.Equal(domainAddress.Latitude, ApplicationAddress.Latitude);
            Assert.Equal(domainAddress.Longitude, ApplicationAddress.Longitude);
            Assert.Equal(domainAddress.AdjustedLatitude, ApplicationAddress.AdjustedLatitude);
            Assert.Equal(domainAddress.AdjustedLongitude, ApplicationAddress.AdjustedLongitude);
            Assert.Equal(domainAddress.FormattedText, ApplicationAddress.FormattedText);
            Assert.Equal(domainAddress.Request, ApplicationAddress.Request);
        }

        [Fact]
        public void FromExternalAddressess()
        {
            var domainAddress1 = new AddressEM
            {
                Country = "Россия",
                Province = "Московская область",
                Area = "Москва",
                Locality = "Москва",
                District = "Северо-Восточный район",
                Street = "4-я Тверская-Ямская улица",
                House = "7",
                Latitude = 55.771899,
                Longitude = 37.597576,
                FormattedText = "Россия, Москва, 4-я Тверская-Ямская улица, 7",
                Request = "Москва 4-я Тверская улица 7"
            };

            var domainAddress2 = new AddressEM
            {
                Country = "Россия",
                Province = "Ярославская область",
                Area = "Ярославль",
                Locality = "Ярославль",
                District = "Северо-Восточный район",
                Street = "4-я Тверская-Ямская улица",
                House = "7",
                Latitude = 55.771899,
                Longitude = 37.597576,
                FormattedText = "Россия, Ярославль, 4-я Тверская-Ямская улица, 7",
                Request = "Ярославль 4-я Тверская улица 7"
            };

            var domainAddressess = new List<AddressEM>
            {
                domainAddress1,
                domainAddress2
            };

            var ApplicationAddressess = Suite.AddressService.FromExternalAddressess(domainAddressess);

            Assert.Equal(domainAddressess.Count, ApplicationAddressess.Count);

            for (int i = 0; i < ApplicationAddressess.Count; i++)
            {
                var domainAddress = domainAddressess[i];
                var ApplicationAddress = ApplicationAddressess.ToList()[i];

                Assert.Equal(domainAddress.Country, ApplicationAddress.Country);
                Assert.Equal(domainAddress.Province, ApplicationAddress.Province);
                Assert.Equal(domainAddress.Area, ApplicationAddress.Area);
                Assert.Equal(domainAddress.Locality, ApplicationAddress.Locality);
                Assert.Equal(domainAddress.District, ApplicationAddress.District);
                Assert.Equal(domainAddress.Street, ApplicationAddress.Street);
                Assert.Equal(domainAddress.House, ApplicationAddress.House);
                Assert.Equal(domainAddress.Latitude, ApplicationAddress.Latitude);
                Assert.Equal(domainAddress.Longitude, ApplicationAddress.Longitude);
                Assert.Equal(domainAddress.FormattedText, ApplicationAddress.FormattedText);
                Assert.Equal(domainAddress.Request, ApplicationAddress.Request);
            }
        }

        [Fact]
        public void FromExternalAddress()
        {
            var domainAddress = new AddressEM
            {
                Country = "Россия",
                Province = "Московская область",
                Area = "Москва",
                Locality = "Москва",
                District = "Северо-Восточный район",
                Street = "4-я Тверская-Ямская улица",
                House = "7",
                Latitude = 55.771899,
                Longitude = 37.597576,
                FormattedText = "Россия, Москва, 4-я Тверская-Ямская улица, 7",
                Request = "Москва 4-я Тверская улица 7"
            };

            var ApplicationAddress = Suite.AddressService.FromExternalAddress(domainAddress);

            Assert.Equal(domainAddress.Country, ApplicationAddress.Country);
            Assert.Equal(domainAddress.Province, ApplicationAddress.Province);
            Assert.Equal(domainAddress.Area, ApplicationAddress.Area);
            Assert.Equal(domainAddress.Locality, ApplicationAddress.Locality);
            Assert.Equal(domainAddress.District, ApplicationAddress.District);
            Assert.Equal(domainAddress.Street, ApplicationAddress.Street);
            Assert.Equal(domainAddress.House, ApplicationAddress.House);
            Assert.Equal(domainAddress.Latitude, ApplicationAddress.Latitude);
            Assert.Equal(domainAddress.Longitude, ApplicationAddress.Longitude);
            Assert.Equal(domainAddress.FormattedText, ApplicationAddress.FormattedText);
            Assert.Equal(domainAddress.Request, ApplicationAddress.Request);
        }

        [Fact]
        public async Task GetTimeBeltByAddress()
        {
            var address = new AddressAM { Latitude = 1, Longitude = 2 };
            var timeBelt = new TimeBelt();

            Suite.MapsServiceMock
                .Setup(m => m.GetTimeBelt(
                    It.Is<Coordinate>(
                        c => c.Latitude.Equals(address.Latitude)
                        && c.Longitude.Equals(address.Longitude))))
                .ReturnsAsync(timeBelt);

            var result = await Suite.AddressService.GetTimeBeltByAddress(address);

            Assert.Equal(timeBelt, result);
        }
    }
}