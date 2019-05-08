using Common.Models.Units;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Pricing;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests.Organization
{
    public class ApplicationMarketServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationMarketServiceTestSuite()
        {
            DomainMarketServiceMock = new Mock<IMarketService>();
            DomainCompanyServiceMock = new Mock<ICompanyService>();
            AddressServiceMock = new Mock<IApplicationAddressService>();
            PricelistServiceMock = new Mock<IApplicationPricelistService>();

            MarketService = new ApplicationMarketService(
                TransactionServiceMock.Object,
                DomainMarketServiceMock.Object,
                DomainCompanyServiceMock.Object,
                AddressServiceMock.Object,
                PricelistServiceMock.Object);
        }

        public IApplicationMarketService MarketService { get; }

        public Mock<IMarketService> DomainMarketServiceMock { get; }

        public Mock<ICompanyService> DomainCompanyServiceMock { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }

        public Mock<IApplicationPricelistService> PricelistServiceMock { get; }
    }

    public class ApplicationMarketServiceTests : BaseServiceTests<ApplicationMarketServiceTestSuite>
    {
        [Fact]
        public async Task CreateDomainMarket()
        {
            var commonId = 1;
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

            var domainCompany = new Company
            {
                Id = commonId++,
                Name = "Транспортные системы"
            };

            var domainPricelist = new Pricelist { Id = commonId++ };

            Suite.AddressServiceMock
                .Setup(m => m.CreateDomainAddress(AddressKind.Market, address))
                .ReturnsAsync(domainAddress);
            Suite.PricelistServiceMock
                .Setup(m => m.CreateDomainPricelist(It.IsAny<PricelistAM>()))
                .ReturnsAsync(domainPricelist);

            await Suite.MarketService.CreateDomainMarket(domainCompany.Id, address);

            Suite.DomainMarketServiceMock
                .Verify(m => m.Create(domainCompany.Id,
                    domainAddress.Id,
                    domainPricelist.Id));
        }

        [Fact]
        public async Task GetDomainMarket()
        {
            var marketId = 1;

            await Suite.MarketService.GetDomainMarket(marketId);

            Suite.DomainMarketServiceMock
                .Verify(m => m.Get(marketId));
        }

        [Fact]
        public async Task GetDomainMarketByCoordinate()
        {
            var coordinate = new Coordinate
            {
                Latitude = 11.000,
                Longitude = 22.000
            };

            await Suite.MarketService.GetDomainMarketByCoordinate(coordinate);

            Suite.DomainMarketServiceMock
                .Verify(m => m.GetByCoordinate(
                    coordinate.Latitude,
                    coordinate.Longitude));
        }

        [Fact]
        public async Task GetNearestDomainMarkets_ValidInput_CorrectLength()
        {
            var commonId = 1;

            var companyId = commonId++;
            var coordinate = new Coordinate { Latitude = 11.0000, Longitude = 22.0000 };
            var marketAddresses = new List<Address>();
            var domainMarkets = new List<Market>
            {
                new Market { Id = commonId++, CompanyId = companyId },
                new Market { Id = commonId++, CompanyId = 777 },
                new Market { Id = commonId++, CompanyId = companyId }
            };

            Suite.DomainCompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);
            Suite.AddressServiceMock
                .Setup(m => m.GetNearestDomainAddresses(AddressKind.Market, coordinate, 500, 0))
                .ReturnsAsync(marketAddresses);
            Suite.DomainMarketServiceMock
                .Setup(m => m.GetByAddressIds(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(domainMarkets);

            var result = await Suite.MarketService.GetNearestDomainMarkets(companyId, coordinate);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetNearestDomainMarkets_ValidInput_CorrectElements()
        {
            var commonId = 1;

            var companyId = commonId++;
            var coordinate = new Coordinate { Latitude = 11.0000, Longitude = 22.0000 };
            var marketAddresses = new List<Address>();
            var domainMarkets = new List<Market>
            {
                new Market { Id = commonId++, CompanyId = companyId },
                new Market { Id = commonId++, CompanyId = 777 },
                new Market { Id = commonId++, CompanyId = companyId }
            };

            Suite.DomainCompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);
            Suite.AddressServiceMock
                .Setup(m => m.GetNearestDomainAddresses(AddressKind.Market, coordinate, 500, 0))
                .ReturnsAsync(marketAddresses);
            Suite.DomainMarketServiceMock
                .Setup(m => m.GetByAddressIds(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(domainMarkets);

            var result = await Suite.MarketService.GetNearestDomainMarkets(companyId, coordinate);

            Assert.Equal(2, result.Count);
            Assert.Equal(companyId, result.ElementAt(0).CompanyId);
            Assert.Equal(companyId, result.ElementAt(1).CompanyId);
        }
    }
}