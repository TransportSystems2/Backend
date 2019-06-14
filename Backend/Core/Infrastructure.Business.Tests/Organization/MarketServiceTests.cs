using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Infrastructure.Business.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Organization
{
    public class MarketServiceTestSuite
    {
        public MarketServiceTestSuite()
        {
            RepositoryMock = new Mock<IMarketRepository>();
            AddressServiceMock = new Mock<IAddressService>();
            PricelistServiceMock = new Mock<IPricelistService>();
            CompanyServiceMock = new Mock<ICompanyService>();

            Service = new MarketService(RepositoryMock.Object,
                CompanyServiceMock.Object,
                AddressServiceMock.Object,
                PricelistServiceMock.Object);
        }
        public IMarketService Service { get; }

        public Mock<IMarketRepository> RepositoryMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<IAddressService> AddressServiceMock { get; }

        public Mock<IPricelistService> PricelistServiceMock { get; }
    }

    public class MarketServiceTests
    {
        public MarketServiceTests()
        {
            Suite = new MarketServiceTestSuite();
        }

        public MarketServiceTestSuite Suite { get; }

        [Fact]
        public async void CreateMarket()
        {
            var commonId = 1;

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

            var result = await Suite.Service.Create(companyId, addressId, pricelistId);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<Market>(
                    g => g.CompanyId.Equals(companyId)
                    && g.AddressId.Equals(addressId)
                    && g.PricelistId.Equals(pricelistId))));
            Suite.RepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(companyId, result.CompanyId);
            Assert.Equal(addressId, result.AddressId);
            Assert.Equal(pricelistId, result.PricelistId);
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

            var market = new Market
            {
                Id = commonId,
                AddressId = address.Id
            };

            Suite.AddressServiceMock
                .Setup(m => m.GetByCoordinate(AddressKind.Market, address.Latitude, address.Longitude))
                .ReturnsAsync(address);
            Suite.RepositoryMock
                .Setup(m => m.GetByAddress(address.Id))
                .ReturnsAsync(market);

            var result = await Suite.Service.GetByCoordinate(address.Latitude, address.Longitude);

            Assert.Equal(market, result);
            Assert.Equal(market.Id, result.Id);
            Assert.Equal(market.AddressId, result.AddressId);
        }
    }
}