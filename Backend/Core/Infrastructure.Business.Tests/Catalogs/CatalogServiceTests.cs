using Moq;
using TransportSystems.Backend.Core.Domain.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Infrastructure.Business.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Catalogs
{
    public class CatalogServiceTestSuite
    {
        public CatalogServiceTestSuite()
        {
            CatalogRepositoryMock = new Mock<ICatalogRepository>();
            CatalogService = new CatalogService(CatalogRepositoryMock.Object);
        }

        public Mock<ICatalogRepository> CatalogRepositoryMock { get; }

        public ICatalogService CatalogService { get; }
    }

    public class CatalogServiceTests
    {
        public CatalogServiceTests()
        {
            Suite = new CatalogServiceTestSuite();
        }

        protected CatalogServiceTestSuite Suite { get; }

        /*
        [Fact]
        public async Task Create()
        {
            var vehicleId = 0;
            var catalogItemId = 1;

            Suite.VehicleServiceMock
                 .Setup(m => m.IsExistEntity(vehicleId))
                 .ReturnsAsync(true);

            Suite.VehicleCatalogItemServiceMock
                 .Setup(m => m.IsExistEntity(catalogItemId))
                 .ReturnsAsync(true);

            var vehicleParameter = await Suite.VehicleParametersService.Create(vehicleId, catalogItemId);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Add(It.Is<VehicleParameter>(
                     p => p.VehicleId.Equals(vehicleId)
                     && p.CatalogItemId.Equals(catalogItemId))), Times.Once);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Save(), Times.Once);

            Assert.Equal(vehicleId, vehicleParameter.VehicleId);
            Assert.Equal(catalogItemId, vehicleParameter.CatalogItemId);
        }

        [Fact]
        public async Task CreateWithNotExistVehicle()
        {
            var vehicleId = 0;
            var catalogItemId = 1;

            await Assert.ThrowsAsync<EntityNotFoundException>("vehicleId", () => Suite.VehicleParametersService.Create(vehicleId, catalogItemId));

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Add(It.Is<VehicleParameter>(p => p.VehicleId.Equals(vehicleId) && p.CatalogItemId.Equals(catalogItemId))), Times.Never);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }

        [Fact]
        public async Task CreateWithNotExistCatalogItem()
        {
            var vehicleId = 0;
            var catalogItemId = 1;

            Suite.VehicleServiceMock
                 .Setup(m => m.IsExistEntity(vehicleId))
                 .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("catalogItemId", () => Suite.VehicleParametersService.Create(vehicleId, catalogItemId));

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Add(It.Is<VehicleParameter>(p => p.VehicleId.Equals(vehicleId) && p.CatalogItemId.Equals(catalogItemId))), Times.Never);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }

        [Fact]
        public async Task GetByVehicle()
        {
            var vehicleId = 0;

            ICollection<VehicleParameter> storedParameters = new List<VehicleParameter>
            {
                new VehicleParameter { VehicleId = vehicleId },
                new VehicleParameter { VehicleId = vehicleId },
                new VehicleParameter { VehicleId = vehicleId }
            };

            Suite.VehicleParametersRepositoryMock
                 .Setup(m => m.ByVehicle(vehicleId))
                 .ReturnsAsync(storedParameters);

            var parameters = await Suite.VehicleParametersService.ByVehicle(vehicleId);

            Assert.Equal(3, parameters.Count);
            Assert.All(parameters, p => p.VehicleId = vehicleId);
        }

        [Fact]
        public async Task GetByCatalogItem()
        {
            var catalogItemId = 0;

            ICollection<VehicleParameter> storedParameters = new List<VehicleParameter>
            {
                new VehicleParameter { CatalogItemId = catalogItemId },
                new VehicleParameter { CatalogItemId = catalogItemId },
                new VehicleParameter { CatalogItemId = catalogItemId }
            };

            Suite.VehicleParametersRepositoryMock
                 .Setup(m => m.ByCatalogItem(catalogItemId))
                 .ReturnsAsync(storedParameters);

            var parameters = await Suite.VehicleParametersService.ByCatalogItem(catalogItemId);

            Assert.Equal(3, parameters.Count);
            Assert.All(parameters, p => p.CatalogItemId = catalogItemId);
        }

        [Fact]
        public async Task Update()
        {
            var parameterId = 0;
            var vehicleId = 1;
            var catalogItemId = 2;

            var newVehicleId = 11;
            var newCatalogItemId = 12;

            var storedParameter = new VehicleParameter
            {
                Id = parameterId,
                VehicleId = vehicleId,
                CatalogItemId = catalogItemId
            };

            Suite.VehicleParametersRepositoryMock
                 .Setup(m => m.Get(parameterId))
                 .ReturnsAsync(storedParameter);

            var parameter = await Suite.VehicleParametersService.Update(parameterId, newVehicleId, newCatalogItemId);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Update(It.Is<VehicleParameter>(
                     p => p.Id.Equals(parameterId)
                     && p.VehicleId.Equals(newVehicleId)
                     && p.CatalogItemId.Equals(newCatalogItemId))), Times.Once);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Save(), Times.Once);

            Assert.Equal(newVehicleId, parameter.VehicleId);
            Assert.Equal(newCatalogItemId, parameter.CatalogItemId);
        }

        [Fact]
        public async Task UpdateNotExistParameter()
        {
            var parameterId = 0;
            var vehicleId = 1;
            var catalogItemId = 2;

            await Assert.ThrowsAsync<EntityNotFoundException>("id", () => Suite.VehicleParametersService.Update(parameterId, vehicleId, catalogItemId));

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Update(It.Is<VehicleParameter>(
                     p => p.Id.Equals(parameterId)
                     && p.VehicleId.Equals(vehicleId)
                     && p.CatalogItemId.Equals(catalogItemId))), Times.Never);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }

        [Fact]
        public async Task Remove()
        {
            var parameterId = 0;
            var vehicleId = 1;
            var catalogItemId = 2;

            var storedParameter = new VehicleParameter
            {
                Id = parameterId,
                VehicleId = vehicleId,
                CatalogItemId = catalogItemId
            };

            Suite.VehicleParametersRepositoryMock
                 .Setup(m => m.Get(parameterId))
                 .ReturnsAsynct(storedParameter);

            var parameter = await Suite.VehicleParametersService.Remove(parameterId);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Remove(It.Is<VehicleParameter>(
                     p => p.Id.Equals(parameterId)
                     && p.VehicleId.Equals(vehicleId)
                     && p.CatalogItemId.Equals(catalogItemId))));

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Save(), Times.Once);

            Assert.Equal(vehicleId, parameter.VehicleId);
            Assert.Equal(catalogItemId, parameter.CatalogItemId);
        }

        [Fact]
        public async Task RemoveNotExistParameter()
        {
            var parameterId = 0;

            await Assert.ThrowsAsync<EntityNotFoundException>("id", () => Suite.VehicleParametersService.Remove(parameterId));

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Remove(It.Is<VehicleParameter>(p => p.Id.Equals(parameterId))), Times.Never);

            Suite.VehicleParametersRepositoryMock
                 .Verify(m => m.Save(), Times.Never);
        }
        */
    }
}