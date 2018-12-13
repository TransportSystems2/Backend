using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Application.Business;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business
{
    public class VehicleServiceTestsSuite : TransactionTestsSuite
    {
        public VehicleServiceTestsSuite()
        {
            CatalogServiceMock = new Mock<IApplicationCatalogService>();
            DomainVehicleServiceMock = new Mock<IVehicleService>();

            VehicleService = new ApplicationVehicleService(
                TransactionServiceMock.Object,
                DomainVehicleServiceMock.Object,
                CatalogServiceMock.Object);
        }

        public Mock<IVehicleService> DomainVehicleServiceMock { get; }

        public Mock<IApplicationCatalogService> CatalogServiceMock { get; }

        public IApplicationVehicleService VehicleService { get; }
    }

    public class ApplicationVehicleServiceTests : BaseServiceTests<VehicleServiceTestsSuite>
    {
        [Fact]
        public async Task GetVehicleCatalogItems()
        {
            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItems(It.IsAny<CatalogKind>(), It.IsAny<CatalogItemKind>()))
                .ReturnsAsync(new List<CatalogItemAM>());

            var result = await Suite.VehicleService.GetCatalogItems();

            Suite.CatalogServiceMock
                .Verify(m => m.GetCatalogItems(CatalogKind.Vehicle, It.IsAny<CatalogItemKind>()), Times.Exactly(3));
        }
    }
}