using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Application.Business;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests
{
    public class ApplicationCargoServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationCargoServiceTestSuite()
        {
            DomainCargoServiceMock = new Mock<ICargoService>();
            CatalogServiceMock = new Mock<IApplicationCatalogService>();
            RegistrationNumberServiceMock = new Mock<IRegistrationNumberService>();

            CargoService = new ApplicationCargoService(
                TransactionServiceMock.Object,
                MappingService,
                DomainCargoServiceMock.Object,
                CatalogServiceMock.Object,
                RegistrationNumberServiceMock.Object);
        }

        public IApplicationCargoService CargoService { get; }

        public Mock<ICargoService> DomainCargoServiceMock { get; }

        public Mock<IApplicationCatalogService> CatalogServiceMock { get; }

        public Mock<IRegistrationNumberService> RegistrationNumberServiceMock { get; }
    }

    public class ApplicationCargoServiceTests : BaseServiceTests<ApplicationCargoServiceTestSuite>
    {
        [Fact]
        public async Task CreateDomainCargo()
        {
            var commonId = 1;

            var cargo = new CargoAM
            {
                WeightCatalogItemId = commonId++,
                KindCatalogItemId = commonId++,
                BrandCatalogItemId = commonId++,
                RegistrationNumber = "к232уу777",
                Comment = "Не работате пневма"
            };

            await Suite.CargoService.CreateDomainCargo(cargo);

            Suite.DomainCargoServiceMock
                .Verify(m => m.Create(
                    cargo.WeightCatalogItemId,
                    cargo.KindCatalogItemId,
                    cargo.BrandCatalogItemId,
                    cargo.RegistrationNumber,
                    cargo.Comment));
        }

        [Fact]
        public async Task CetCargo()
        {
            var commonId = 1;
            var cargoId = commonId++;

            var domainCargo = new Cargo
            {
                WeightCatalogItemId = commonId++,
                KindCatalogItemId = commonId++,
                BrandCatalogItemId = commonId++,
                RegistrationNumber = "е111кк778",
                Comment = "Не работает пневма"
            };

            Suite.DomainCargoServiceMock
                .Setup(m => m.Get(cargoId))
                .ReturnsAsync(domainCargo);

            var result = await Suite.CargoService.GetCargo(cargoId);

            Assert.Equal(domainCargo.WeightCatalogItemId, result.WeightCatalogItemId);
            Assert.Equal(domainCargo.KindCatalogItemId, result.KindCatalogItemId);
            Assert.Equal(domainCargo.BrandCatalogItemId, result.BrandCatalogItemId);
            Assert.Equal(domainCargo.RegistrationNumber, result.RegistrationNumber);
            Assert.Equal(domainCargo.Comment, result.Comment);
        }

        [Fact]
        public async Task GetCargoCatalogItems()
        {
            Suite.CatalogServiceMock
                .Setup(m => m.GetCatalogItems(It.IsAny<CatalogKind>(), It.IsAny<CatalogItemKind>()))
                .ReturnsAsync(new List<CatalogItemAM>());

            var result = await Suite.CargoService.GetCatalogItems();

            Suite.CatalogServiceMock
                .Verify(m => m.GetCatalogItems(CatalogKind.Cargo, It.IsAny<CatalogItemKind>()), Times.Exactly(3));
        }

        [Fact]
        public async Task ValidRegistrationNumber()
        {
            var registrationNumber = "х827мн76";
            var result = await Suite.CargoService.ValidRegistrationNumber(registrationNumber);

            Suite.RegistrationNumberServiceMock.Verify(
                m => m.ValidRegistrationNumber(registrationNumber),
                Times.Once);
        }
    }
}