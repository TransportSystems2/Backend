﻿using System;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Application.Business.Catalogs;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests.Catalogs
{
    public class ApplicationCatalogServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationCatalogServiceTestSuite()
        {
            DomainCatalogServiceMock = new Mock<ICatalogService>();
            DomainCatalogItemServiceMock = new Mock<ICatalogItemService>();

            CatalogService = new ApplicationCatalogService(
                TransactionServiceMock.Object,
                MappingService,
                DomainCatalogServiceMock.Object,
                DomainCatalogItemServiceMock.Object);
        }

        public IApplicationCatalogService CatalogService { get; }

        public Mock<ICatalogService> DomainCatalogServiceMock { get; }

        public Mock<ICatalogItemService> DomainCatalogItemServiceMock { get; }
    }

    public class ApplicationCatalogServiceTests : BaseServiceTests<ApplicationCatalogServiceTestSuite>
    {
        [Fact]
        public async Task CreateCatalogItem()
        {
            var commonId = 1;

            var catalogItem = new CatalogItemAM
            {
                Kind = CatalogItemKind.Brand,
                Name = "Запорожец",
                Value = 0
            };

            var catalogId = commonId++;

            var domainCatalogItem = new CatalogItem
            {
                Id = commonId++,
                Kind = catalogItem.Kind,
                Name = catalogItem.Name,
                Value = catalogItem.Value
            };

            Suite.DomainCatalogItemServiceMock
                .Setup(m => m.Create(catalogId, catalogItem.Kind, catalogItem.Name, catalogItem.Value))
                .ReturnsAsync(domainCatalogItem);

            var result = await Suite.CatalogService.CreateCatalogItem(catalogId, catalogItem);

            Assert.Equal(domainCatalogItem.Id, result.Id);
            Assert.Equal(domainCatalogItem.Kind, result.Kind);
            Assert.Equal(domainCatalogItem.Name, result.Name);
            Assert.Equal(domainCatalogItem.Value, result.Value);
        }

        [Fact]
        public async Task GetCatalogItem()
        {
            var commonId = 0;

            var catalogItem = new CatalogItem
            {
                Id = commonId++,
                Name = "SomeName",
                Kind = CatalogItemKind.Weight,
                Value = 4
            };

            Suite.DomainCatalogItemServiceMock
                .Setup(m => m.Get(catalogItem.Id))
                .ReturnsAsync(catalogItem);

            var result = await Suite.CatalogService.GetCatalogItem(catalogItem.Id);

            Assert.Equal(catalogItem.Id, result.Id);
            Assert.Equal(catalogItem.Name, result.Name);
            Assert.Equal(catalogItem.Kind, result.Kind);
            Assert.Equal(catalogItem.Value, result.Value);
        }

        [Fact]
        public async Task GetCatalogItem_When_CatalogItemDoesNotExist_Result_ArgumentException()
        {
            var notExistingCatalogItemId = 1;

            Suite.DomainCatalogItemServiceMock
                .Setup(m => m.Get(notExistingCatalogItemId))
                .Returns(Task.FromResult<CatalogItem>(null));

            await Assert.ThrowsAsync<ArgumentException>(
                "CatalogItem",
                () => Suite.CatalogService.GetCatalogItem(notExistingCatalogItemId));
        }
    }
}