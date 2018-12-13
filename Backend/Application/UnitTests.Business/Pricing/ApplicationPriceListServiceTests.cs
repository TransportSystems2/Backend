using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using TransportSystems.Backend.Application.Business.Pricing;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.Models.Pricing;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Pricing
{
    public class PricelistServiceTestsTestsSuite : TransactionTestsSuite
    {
        public PricelistServiceTestsTestsSuite()
        {
            DomainPricelistServiceMock = new Mock<IPricelistService>();
            DomainPriceServiceMock = new Mock<IPriceService>();
            CatalogServiceMock = new Mock<IApplicationCatalogService>();

            PricelistService = new ApplicationPricelistService(
                TransactionServiceMock.Object,
                DomainPricelistServiceMock.Object,
                DomainPriceServiceMock.Object,
                CatalogServiceMock.Object);
        }

        public IApplicationPricelistService PricelistService { get; }

        public Mock<IPricelistService> DomainPricelistServiceMock { get; }

        public Mock<IPriceService> DomainPriceServiceMock { get; }

        public Mock<IApplicationCatalogService> CatalogServiceMock { get; }
    }

    public class ApplicationPricelistServiceTests : BaseServiceTests<PricelistServiceTestsTestsSuite>
    {
        [Fact]
        public async Task CreatePricelistBlank()
        {
            var catalogKind = CatalogKind.Cargo;
            var catalogItemKind = CatalogItemKind.Weight;
            var catalogItems = new List<CatalogItemAM>
            {
                new CatalogItemAM {
                    Id = 4,
                    Name = "0.5t",
                    Value = 500
                },
                new CatalogItemAM {
                    Id = 5,
                    Name = "1t",
                    Value = 1000
                },
                new CatalogItemAM {
                    Id = 6,
                    Name = "1.5t",
                    Value = 1500
                }
            };

            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItems(catalogKind, catalogItemKind))
                .ReturnsAsync(catalogItems);

            var result = await Suite.PricelistService.CreatePricelistBlank();

            Assert.Equal(catalogItems.Count, result.Items.Count);

            foreach (var price in result.Items)
            {
                var catalog = catalogItems[result.Items.IndexOf(price)];
                Assert.Equal(catalog.Id, price.CatalogItemId);
                Assert.Equal(catalog.Name, price.Name);
                Assert.Equal(Price.DefaultComissionPercentage, price.CommissionPercentage);
            }
        }

        [Fact]
        public async Task CreatePricelist()
        {
            var catalogKind = CatalogKind.Cargo;
            var catalogItemKind = CatalogItemKind.Weight;
            var catalogItems = new List<CatalogItemAM>
            {
                new CatalogItemAM {
                    Id = 4,
                    Name = "0.5t",
                    Value = 500
                },
                new CatalogItemAM {
                    Id = 5,
                    Name = "1t",
                    Value = 1000
                },
                new CatalogItemAM {
                    Id = 6,
                    Name = "1.5t",
                    Value = 1500
                }
            };

            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItems(catalogKind, catalogItemKind))
                .ReturnsAsync(catalogItems);

            var pricelist = new PricelistAM
            {
                Items =
                {
                    new PriceAM { CatalogItemId = catalogItems[0].Id },
                    new PriceAM { CatalogItemId = catalogItems[1].Id },
                    new PriceAM { CatalogItemId = catalogItems[2].Id }
                }
            };

            var domainPricelist = new Pricelist { Id = 1 };
            Suite.DomainPricelistServiceMock
                .Setup(m => m.Create())
                .ReturnsAsync(domainPricelist);

            var result = await Suite.PricelistService.CreateDomainPricelist();

            Suite.DomainPriceServiceMock
                .Verify(
                    m => m.Create(
                        domainPricelist.Id,
                        It.IsInRange(4, 6, Range.Inclusive),
                        It.IsAny<string>(),
                        It.IsAny<byte>(),
                        It.IsAny<decimal>(),
                        It.IsAny<decimal>(),
                        It.IsAny<decimal>(), 
                        It.IsAny<decimal>(),
                        It.IsAny<decimal>(),
                        It.IsAny<decimal>()),
                    Times.Exactly(pricelist.Items.Count));
        }

        [Fact]
        public async Task CreatePrice()
        {
            var commonId = 1;

            var pricelistId = commonId++;
            var price = new PriceAM
            {
                CatalogItemId = commonId++,
                Name = "name",
                CommissionPercentage = 10,
                PerKm = 1m,
                Loading = 2m,
                LockedSteering = 3m,
                LockedWheel = 4m,
                Overturned = 5m,
                Ditch = 6m
            };

            var domainPrice = await Suite.PricelistService.CreateDomainPrice(pricelistId, price);

            Suite.DomainPriceServiceMock
                .Verify(m => m.Create(
                    pricelistId,
                    price.CatalogItemId,
                    price.Name,
                    price.CommissionPercentage,
                    price.PerKm,
                    price.Loading,
                    price.LockedSteering,
                    price.LockedWheel,
                    price.Overturned,
                    price.Ditch));
        }

        [Fact]
        public async Task GetDomainPriceByPricelistAndCatalogItem()
        {
            var pricelistId = 1;
            var domainCatalogItemId = 2;

            var result = await Suite.PricelistService.GetDomainPrice(pricelistId, domainCatalogItemId);

            Suite.DomainPriceServiceMock
                .Verify(m => m.Get(pricelistId, domainCatalogItemId));
        }

        [Fact]
        public async Task GetDomainPriceById()
        {
            var priceId = 1;
            await Suite.PricelistService.GetDomainPrice(priceId);

            Suite.DomainPriceServiceMock
                .Verify(m => m.Get(priceId));
        }
    }
}