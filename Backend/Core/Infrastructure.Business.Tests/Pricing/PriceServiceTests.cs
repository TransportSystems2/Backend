using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;
using TransportSystems.Backend.Core.Infrastructure.Business.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Pricing
{
    public class PriceServiceTestSuite
    {
        public PriceServiceTestSuite()
        {
            PriceRepositoryMock = new Mock<IPriceRepository>();
            PricelistServiceMock = new Mock<IPricelistService>();
            CatalogItemServiceMock = new Mock<ICatalogItemService>();

            PriceService = new PriceService(
                PriceRepositoryMock.Object,
                PricelistServiceMock.Object,
                CatalogItemServiceMock.Object);
        }

        public Mock<IPricelistService> PricelistServiceMock { get; }

        public Mock<ICatalogItemService> CatalogItemServiceMock { get; }

        public Mock<IPriceRepository> PriceRepositoryMock { get; }

        public IPriceService PriceService { get; }
    }

    public class PriceServiceTests
    {
        public PriceServiceTests()
        {
            Suite = new PriceServiceTestSuite();
        }

        public PriceServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreatePrice()
        {
            var pricelist = new Pricelist { Id = 1 };
            var catalogItem = new CatalogItem { Id = 2, Kind = CatalogItemKind.Weight, Name = "1t", Value = 1000 };

            var name = "some name";
            byte commissionPercentage = 10;
            var perMeter = 1m;
            var loading = 2m;
            var lockedSteering = 3m;
            var lockedWheel = 4m;
            var overturned = 5m;
            var ditch = 6m;

            Suite.PricelistServiceMock
                .Setup(m => m.Get(pricelist.Id))
                .ReturnsAsync(pricelist);
            Suite.PricelistServiceMock
                .Setup(m => m.IsExist(pricelist.Id))
                .ReturnsAsync(true);
            Suite.CatalogItemServiceMock
                .Setup(m => m.Get(catalogItem.Id))
                .ReturnsAsync(catalogItem);
            Suite.CatalogItemServiceMock
                .Setup(m => m.IsExist(catalogItem.Id))
                .ReturnsAsync(true);
            Suite.PriceRepositoryMock
                .Setup(m => m.Get(pricelist.Id, catalogItem.Id))
                .Returns(Task.FromResult<Price>(null));

            var result = await Suite.PriceService.Create(
                pricelist.Id,
                catalogItem.Id,
                name,
                commissionPercentage,
                perMeter,
                loading,
                lockedSteering,
                lockedWheel,
                overturned,
                ditch);

            Suite.PriceRepositoryMock
                .Verify(m => m.Add(It.Is<Price>(
                    p => p.PricelistId.Equals(pricelist.Id)
                    && p.CatalogItemId.Equals(catalogItem.Id)
                    && p.Name.Equals(name)
                    && p.CommissionPercentage.Equals(commissionPercentage)
                    && p.PerMeter.Equals(perMeter)
                    && p.Loading.Equals(loading)
                    && p.LockedSteering.Equals(lockedSteering)
                    && p.LockedWheel.Equals(lockedWheel)
                    && p.Overturned.Equals(overturned)
                    && p.Ditch.Equals(ditch))));

            Suite.PriceRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(pricelist.Id, result.PricelistId);
            Assert.Equal(catalogItem.Id, result.CatalogItemId);
            Assert.Equal(name, result.Name);
            Assert.Equal(commissionPercentage, result.CommissionPercentage);
            Assert.Equal(perMeter, result.PerMeter);
            Assert.Equal(loading, result.Loading);
            Assert.Equal(lockedSteering, result.LockedSteering);
            Assert.Equal(lockedWheel, result.LockedWheel);
            Assert.Equal(overturned, result.Overturned);
            Assert.Equal(ditch, result.Ditch);
        }

        [Fact]
        public async Task CreatePriceWhenPricelistIdDidNotFound()
        {
            var notExistingPricelistId = 1;

            await Assert.ThrowsAsync<EntityNotFoundException>("Pricelist", 
                () => Suite.PriceService.Create(
                    notExistingPricelistId,
                    2,
                    "name",
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0));
        }

        [Fact]
        public async Task CreatePriceWhenCatalogItemIdIsNotFound()
        {
            var pricelist = new Pricelist { Id = 1 };
            var notExistingCatalogItemId = 2;

            Suite.PricelistServiceMock
                .Setup(m => m.Get(pricelist.Id))
                .ReturnsAsync(pricelist);
            Suite.PricelistServiceMock
                .Setup(m => m.IsExist(pricelist.Id))
                .ReturnsAsync(true);
            Suite.CatalogItemServiceMock
                .Setup(m => m.Get(notExistingCatalogItemId))
                .Returns(Task.FromResult<CatalogItem>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "CatalogItem",
                () => Suite.PriceService.Create(
                    pricelist.Id,
                    notExistingCatalogItemId,
                    "name",
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0));
        }

        [Fact]
        public async Task CreatePriceWhenPriceWithSameParametersAlreadyExists()
        {
            var pricelist = new Pricelist { Id = 1 };
            var catalogItem = new CatalogItem { Id = 2 };

            var existingPrice = new Price { Id = 3, PricelistId = pricelist.Id, CatalogItemId = catalogItem.Id };

            Suite.PricelistServiceMock
                .Setup(m => m.Get(pricelist.Id))
                .ReturnsAsync(pricelist);
            Suite.CatalogItemServiceMock
                .Setup(m => m.Get(catalogItem.Id))
                .ReturnsAsync(catalogItem);
            Suite.PriceRepositoryMock
                .Setup(m => m.Get(pricelist.Id, catalogItem.Id))
                .ReturnsAsync(existingPrice);

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(
                "Price",
                () => Suite.PriceService.Create(
                    pricelist.Id,
                    catalogItem.Id,
                    "name",
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0));
        }
    }
}