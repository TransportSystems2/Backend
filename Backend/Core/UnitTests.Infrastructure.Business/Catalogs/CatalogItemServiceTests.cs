using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Infrastructure.Business.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Catalogs
{
    public class CatalogItemServiceTestSuite
    {
        public CatalogItemServiceTestSuite()
        {
            CatalogItemRepositoryMock = new Mock<ICatalogItemRepository>();
            CatalogServiceMock = new Mock<ICatalogService>();
            CatalogItemService = new CatalogItemService(
                CatalogItemRepositoryMock.Object,
                CatalogServiceMock.Object);
        }

        public Mock<ICatalogItemRepository> CatalogItemRepositoryMock { get; }

        public Mock<ICatalogService> CatalogServiceMock { get; }

        public ICatalogItemService CatalogItemService { get; }
    }

    public class CatalogItemServiceTests
    {
        public CatalogItemServiceTests()
        {
            Suite = new CatalogItemServiceTestSuite();
        }

        protected CatalogItemServiceTestSuite Suite { get; }

        [Fact]
        public async Task Create()
        {
            var catalog = new Catalog { Id = 1 };

            var kind = CatalogItemKind.Brand;
            var name = "Мерседес";
            var value = 0;

            Suite.CatalogServiceMock
                 .Setup(m => m.IsExist(catalog.Id))
                 .ReturnsAsync(true);

            var catalogItem = await Suite.CatalogItemService.Create(
                catalog.Id,
                kind,
                name,
                value);

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.Add(It.Is<CatalogItem>(
                     p => p.CatalogId.Equals(catalog.Id)
                     && p.Kind.Equals(kind)
                     && p.Name.Equals(name)
                     && p.Value.Equals(value))), Times.Once);

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.Save(), Times.Once);

            Assert.Equal(catalog.Id, catalogItem.CatalogId);
            Assert.Equal(kind, catalogItem.Kind);
            Assert.Equal(name, catalogItem.Name);
            Assert.Equal(value, catalogItem.Value);
        }

        [Fact]
        public async Task CreateWhenCatalogIdDoesNotExist()
        {
            var notExistingCatalogId = 1;

            var kind = CatalogItemKind.Brand;
            var name = "Мерседес";
            var value = 0;

            Suite.CatalogServiceMock
                 .Setup(m => m.IsExist(notExistingCatalogId))
                 .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Catalog",
                () => Suite.CatalogItemService.Create(notExistingCatalogId, kind, name, value));

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.Add(It.IsAny<CatalogItem>()), Times.Never);

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }

        [Fact]
        public async Task CreateWithEmptyName()
        {
            var catalog = new Catalog { Id = 1 };

            var kind = CatalogItemKind.Brand;
            var value = 0;

            Suite.CatalogServiceMock
                 .Setup(m => m.IsExist(catalog.Id))
                 .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentNullException>(
                "Name",
                () => Suite.CatalogItemService.Create(catalog.Id, kind, null, value));

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.Add(It.IsAny<CatalogItem>()), Times.Never);

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }

        [Fact]
        public async Task GetById()
        {
            var itemId = 0;

            Suite.CatalogItemRepositoryMock
                 .Setup(m => m.Get(itemId))
                 .ReturnsAsync(new CatalogItem { Id = itemId });

            await Suite.CatalogItemService.Get(itemId);

            Suite.CatalogItemRepositoryMock
                .Verify(m => m.Get(itemId));
        }

        [Fact]
        public async Task GetByKind()
        {
            var kind = CatalogItemKind.Capacity;
            var catalog = new Catalog
            {
                Id = 1,
                Kind = CatalogKind.Cargo
            };

            Suite.CatalogServiceMock
                 .Setup(m => m.GetByKind(catalog.Kind))
                 .ReturnsAsync(catalog);

            await Suite.CatalogItemService.GetByKind(catalog.Kind, kind);

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.GetByKind(catalog.Id, kind));
        }

        [Fact]
        public async Task GetByParameterIdWithEntityNotFoundException()
        {
            var notExistingCatalogKind = CatalogKind.Vehicle;

            var kind = CatalogItemKind.Brand;
            
            Suite.CatalogServiceMock
                 .Setup(m => m.GetByKind(notExistingCatalogKind))
                 .Returns(Task.FromResult<Catalog>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "catalogKind",
                () => Suite.CatalogItemService.GetByKind(notExistingCatalogKind, kind));

            Suite.CatalogItemRepositoryMock
                 .Verify(m => m.GetByKind(It.IsAny<int>(), kind), Times.Never);
        }
    }
}