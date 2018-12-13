using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Infrastructure.Business.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Oraganization
{
    public class CityServiceTestSuite
    {
        public CityServiceTestSuite()
        {
            CityRepositoryMock = new Mock<ICityRepository>();
            AddressServiceMock = new Mock<IAddressService>();
            PricelistServiceMock = new Mock<IPricelistService>();

            CityService = new CityService(
                CityRepositoryMock.Object,
                AddressServiceMock.Object,
                PricelistServiceMock.Object);
        }
        public Mock<ICityRepository> CityRepositoryMock { get; }

        public Mock<IAddressService> AddressServiceMock { get; }

        public Mock<IPricelistService> PricelistServiceMock { get; }

        public ICityService CityService { get; }
    }

    public class CityServiceTests
    {
        public CityServiceTests()
        {
            Suite = new CityServiceTestSuite();
        }

        protected CityServiceTestSuite Suite { get; }

        [Fact]
        public async void CreateCity()
        {
            var commonId = 1;
            var domain = "rybinsk";
            var addressId = commonId++;
            var pricelistId = commonId++;

            Suite.AddressServiceMock
                .Setup(m => m.IsExist(addressId))
                .ReturnsAsync(true);

            Suite.PricelistServiceMock
                .Setup(m => m.IsExist(pricelistId))
                .ReturnsAsync(true);

            var result = await Suite.CityService.Create(domain, addressId, pricelistId);

            Suite.CityRepositoryMock
                .Verify(m => m.Add(It.Is<City>(
                    c => c.Domain.Equals(domain)
                    && c.AddressId.Equals(addressId)
                    && c.PricelistId.Equals(pricelistId))));
            Suite.CityRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(domain, result.Domain);
            Assert.Equal(addressId, result.AddressId);
        }

        [Fact]
        public async void CreateCityWhenDomainAlreadyExists()
        {
            var commonId = 1;
            var existingCity = new City { Id = commonId++, Domain = "someDomain" };
            var pricelistId = commonId++;
            var addressId = commonId++;

            Suite.CityRepositoryMock
                .Setup(m => m.GetByDomain(It.IsAny<string>()))
                .ReturnsAsync(existingCity);
            Suite.PricelistServiceMock
                .Setup(m => m.IsExist(pricelistId))
                .ReturnsAsync(true);
            Suite.AddressServiceMock
                .Setup(m => m.IsExist(addressId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(
                "Domain",
                () => Suite.CityService.Create(existingCity.Domain, addressId, pricelistId));
        }

        [Fact]
        public async void GetCityByAddress()
        {
            var addressId = 1;
            var city = new City();

            Suite.AddressServiceMock
                .Setup(m => m.IsExist(addressId))
                .ReturnsAsync(true);
            Suite.CityRepositoryMock
                .Setup(m => m.GetByAddress(addressId))
                .ReturnsAsync(city);

            var result = await Suite.CityService.GetByAddress(addressId);

            Assert.Equal(city, result);
        }

        [Fact]
        public async Task IsExistByDomain()
        {
            var domain = "moscow";
            var isExist = true;

            Suite.CityRepositoryMock
                .Setup(m => m.IsExistByDomain(domain))
                .ReturnsAsync(isExist);

            var result = await Suite.CityService.IsExistByDomain(domain);

            Assert.True(result);
        }

        [Fact]
        public async Task IsExistByDomainWhereDomainIsNull()
        {
            await Assert.ThrowsAsync<ArgumentException>("Domain", () => Suite.CityService.IsExistByDomain(null));
        }
    }
}