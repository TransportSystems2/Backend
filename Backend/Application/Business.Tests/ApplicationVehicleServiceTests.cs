using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests
{
    public class VehicleServiceTestsSuite : TransactionTestsSuite
    {
        public VehicleServiceTestsSuite()
        {
            CatalogServiceMock = new Mock<IApplicationCatalogService>();
            DomainVehicleServiceMock = new Mock<IVehicleService>();

            ServiceMock = new Mock<ApplicationVehicleService>(
                TransactionServiceMock.Object,
                DomainVehicleServiceMock.Object,
                CatalogServiceMock.Object);
            ServiceMock.CallBase = true;
        }

        public IApplicationVehicleService Service => ServiceMock.Object;

        public Mock<ApplicationVehicleService> ServiceMock { get; }

        public Mock<IVehicleService> DomainVehicleServiceMock { get; }

        public Mock<IApplicationCatalogService> CatalogServiceMock { get; }
    }

    public class ApplicationVehicleServiceTests : BaseServiceTests<VehicleServiceTestsSuite>
    {
        [Fact]
        public async Task GetVehicleCatalogItems()
        {
            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItems(It.IsAny<CatalogKind>(), It.IsAny<CatalogItemKind>()))
                .ReturnsAsync(new List<CatalogItemAM>());

            var result = await Suite.Service.GetCatalogItems();

            Suite.CatalogServiceMock
                .Verify(m => m.GetCatalogItems(CatalogKind.Vehicle, It.IsAny<CatalogItemKind>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetVehicleByDomainEntity()
        {
            var commonId = 1;

            var brandCatalogItem = new CatalogItemAM { Id = commonId++ };
            var capacityCatalogItem = new CatalogItemAM { Id = commonId++ };
            var kindCatalogItem = new CatalogItemAM { Id = commonId++ };

            var domainVehicle = new Vehicle
            {
                Id = commonId++,
                BrandCatalogItemId = brandCatalogItem.Id,
                CapacityCatalogItemId = capacityCatalogItem.Id,
                KindCatalogItemId = kindCatalogItem.Id,
                RegistrationNumber = "123456"
            };

            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItem(domainVehicle.BrandCatalogItemId))
                .ReturnsAsync(brandCatalogItem);
            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItem(domainVehicle.CapacityCatalogItemId))
                .ReturnsAsync(capacityCatalogItem);
            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItem(domainVehicle.KindCatalogItemId))
                .ReturnsAsync(kindCatalogItem);

            var result = await Suite.Service.GetVehicle(domainVehicle);

            Assert.Equal(domainVehicle.Id, result.Id);
            Assert.Equal(domainVehicle.RegistrationNumber, result.RegistrationNumber);
            Assert.Equal(brandCatalogItem, result.BrandCatalogItem);
            Assert.Equal(capacityCatalogItem, result.CapacityCatalogItem);
            Assert.Equal(kindCatalogItem, result.KindCatalogItem);
        }

        [Fact]
        public async Task GetByCompany_Result_ApplicationVehicleNumberCorrespondDomainNumber()
        {
            var commonId = 1;
            var companyId = commonId++;

            var domainVehicles = new List<Vehicle> {
                new Vehicle { Id = commonId++, CompanyId = companyId },
                new Vehicle { Id = commonId++, CompanyId = companyId }
            };

            Suite.DomainVehicleServiceMock
                .Setup(m => m.GetByCompany(companyId))
                .ReturnsAsync(domainVehicles);
            Suite.ServiceMock
                .Setup(m => m.GetVehicle(It.IsAny<Vehicle>()))
                .ReturnsAsync(new VehicleAM());

            var result = await Suite.Service.GetByCompany(companyId);

            Assert.Equal(domainVehicles.Count, result.Count);
        }
    }
}