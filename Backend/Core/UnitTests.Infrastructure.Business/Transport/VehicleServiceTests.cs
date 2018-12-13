using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;
using TransportSystems.Backend.Core.Infrastructure.Business.Transport;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Transport
{
    public class VehicleServiceTestSuite
    {
        public VehicleServiceTestSuite()
        {
            VehicleRepositoryMock = new Mock<IVehicleRepository>();
            CompanyServiceMock = new Mock<ICompanyService>();
            CatalogItemServiceMock = new Mock<ICatalogItemService>();
            RegistrationNumberServiceMock = new Mock<IRegistrationNumberService>();

            VehicleService = new VehicleService(
                VehicleRepositoryMock.Object,
                CompanyServiceMock.Object,
                CatalogItemServiceMock.Object,
                RegistrationNumberServiceMock.Object);
        }

        public IVehicleService VehicleService { get; }

        public Mock<IVehicleRepository> VehicleRepositoryMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<ICatalogItemService> CatalogItemServiceMock { get; }

        public Mock<IRegistrationNumberService> RegistrationNumberServiceMock { get; }
    }

    public class VehicleServiceTests
    {
        public VehicleServiceTests()
        {
            Suite = new VehicleServiceTestSuite();
        }

        private VehicleServiceTestSuite Suite { get; }

        [Fact]
        public async void Create()
        {
            var companyId = 0;
            var registrationNumber = "Н756АУ76";
            var brandCatalogItem = new CatalogItem
            {
                Id = 1,
                Kind = CatalogItemKind.Brand
            };

            var capacityCatalogItem = new CatalogItem
            {
                Id = 2,
                Kind = CatalogItemKind.Capacity
            };

            var kindCatalogItem = new CatalogItem
            {
                Id = 3,
                Kind = CatalogItemKind.Kind
            };

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);

            Suite.CatalogItemServiceMock
                .Setup(m => m.Get(brandCatalogItem.Id))
                .ReturnsAsync(brandCatalogItem);

            Suite.CatalogItemServiceMock
                .Setup(m => m.Get(capacityCatalogItem.Id))
                .ReturnsAsync(capacityCatalogItem);

            Suite.CatalogItemServiceMock
                .Setup(m => m.Get(kindCatalogItem.Id))
                .ReturnsAsync(kindCatalogItem);

            Suite.RegistrationNumberServiceMock
                 .Setup(m => m.ValidRegistrationNumber(registrationNumber))
                 .ReturnsAsync(true);

            var vehicle = await Suite.VehicleService.Create(
                companyId,
                registrationNumber,
                brandCatalogItem.Id,
                capacityCatalogItem.Id,
                kindCatalogItem.Id);

            Suite.VehicleRepositoryMock
                 .Verify(m => m.Add(It.Is<Vehicle>(
                     v => v.CompanyId.Equals(companyId)
                     && v.RegistrationNumber.Equals(registrationNumber)
                     && v.BrandCatalogItemId.Equals(brandCatalogItem.Id)
                     && v.CapacityCatalogItemId.Equals(capacityCatalogItem.Id)
                     && v.KindCatalogItemId.Equals(kindCatalogItem.Id))));

            Suite.VehicleRepositoryMock
                 .Verify(m => m.Save());

            Assert.Equal(companyId, vehicle.CompanyId);
            Assert.Equal(registrationNumber, vehicle.RegistrationNumber);
            Assert.Equal(brandCatalogItem.Id, vehicle.BrandCatalogItemId);
            Assert.Equal(capacityCatalogItem.Id, vehicle.CapacityCatalogItemId);
            Assert.Equal(kindCatalogItem.Id, vehicle.KindCatalogItemId);
        }

        [Fact]
        public async void CreateWithNotExistCompanyId()
        {
            var companyId = 0;
            var registrationNumber = "Н756АУ76";

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                 .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "CompanyId", 
                () => Suite.VehicleService.Create(companyId, registrationNumber, 1, 1, 1));

            Suite.VehicleRepositoryMock
                 .Verify(m => m.Add(It.IsAny<Vehicle>()), Times.Never);

            Suite.VehicleRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }

        [Fact]
        public async void CreateWithNotValidRegistrationNumber()
        {
            var companyId = 0;
            var registrationNumber = "н756аУ76";

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);
            
            await Assert.ThrowsAsync<ArgumentException>(
                "RegistrationNumber",
                () => Suite.VehicleService.Create(companyId, registrationNumber, 1, 1, 1));

            Suite.VehicleRepositoryMock
                 .Verify(m => m.Add(It.IsAny<Vehicle>()), Times.Never);

            Suite.VehicleRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }
    }
}