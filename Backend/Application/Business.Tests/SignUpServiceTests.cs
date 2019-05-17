using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Business;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.SignUp;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests
{
    public class SignUpServiceTestsSuite : TransactionTestsSuite
    {
        public SignUpServiceTestsSuite()
        {
            DomainDriverServiceMock = new Mock<IDriverService>();
            DomainModeratorServiceMock = new Mock<IModeratorService>();
            DomainDispatcherServiceMock = new Mock<IDispatcherService>();
            DomainCompanyServiceMock = new Mock<ICompanyService>();
            DomainGarageServiceMock = new Mock<IGarageService>();
            VehicleServiceMock = new Mock<IApplicationVehicleService>();

            SignUpService = new SignUpService(
                TransactionServiceMock.Object,
                DomainDriverServiceMock.Object,
                DomainModeratorServiceMock.Object,
                DomainDispatcherServiceMock.Object,
                DomainCompanyServiceMock.Object,
                DomainGarageServiceMock.Object,
                VehicleServiceMock.Object);
        }

        public ISignUpService SignUpService { get; }

        public Mock<IDriverService> DomainDriverServiceMock { get; }

        public Mock<IModeratorService> DomainModeratorServiceMock { get; }

        public Mock<IDispatcherService> DomainDispatcherServiceMock { get; }

        public Mock<ICompanyService> DomainCompanyServiceMock { get; }

        public Mock<IGarageService> DomainGarageServiceMock { get; }

        public Mock<IApplicationVehicleService> VehicleServiceMock { get; }
    }

    public class SignUpServiceTests : BaseServiceTests<SignUpServiceTestsSuite>
    {
        [Fact]
        public async Task SignUpCompany()
        {
            var commonId = 1;

            var dispatcher = new DispatcherAM
            {
                FirstName = "Петя",
                LastName = "Лиссерман",
                PhoneNumber = "+79998887766"
            };

            var garageAddress = new AddressAM
            {
                Country = "Россия",
                Province = "Ярославская",
                Locality = "Рыбинск",
                District = "Центральный"
            };

            var vehicle = new VehicleAM
            {
                RegistrationNumber = "К100ЕЕ77",
                BrandCatalogItemId = commonId++,
                CapacityCatalogItemId = commonId++,
                KindCatalogItemId = commonId++
            };

            var driver = new DriverAM
            {
                FirstName = "Антон",
                LastName = "Шипачев",
                PhoneNumber = "78887775533"
            };

            var dispatcherCompanyModel = new CompanyApplicationAM
            {
                Dispatcher = dispatcher,
                GarageAddress = garageAddress,
                Vehicle = vehicle,
                Driver = driver
            };

            var domainCompany = new Company { Id = commonId++ };
            var domainModerator = new Moderator { Id = commonId++ };
            var domainDispatcher = new Dispatcher { Id = commonId++ };
            var domainDriver = new Driver { Id = commonId++ };
            var domainVehicle = new Vehicle { Id = commonId++ };

            Suite.DomainCompanyServiceMock
                 .Setup(m => m.Create(dispatcherCompanyModel.Dispatcher.PhoneNumber))
                 .ReturnsAsync(domainCompany);

            Suite.DomainDispatcherServiceMock
                 .Setup(m => m.Create(
                     dispatcher.FirstName,
                     dispatcher.LastName,
                     dispatcher.PhoneNumber,
                     domainCompany.Id))
                 .ReturnsAsync(domainDispatcher);

            Suite.VehicleServiceMock
                 .Setup(m => m.CreateDomainVehicle(
                     domainCompany.Id,
                     vehicle))
                 .ReturnsAsync(domainVehicle);

            Suite.DomainDriverServiceMock
                 .Setup(m => m.Create(
                     driver.FirstName,
                     driver.LastName,
                     driver.PhoneNumber,
                     domainCompany.Id))
                 .ReturnsAsync(domainDriver);

            Suite.TransactionServiceMock
                 .Setup(m => m.BeginTransaction())
                 .ReturnsAsync(Suite.TransactionMock.Object);

            await Suite.SignUpService.SignUpCompany(dispatcherCompanyModel);

            Suite.DomainGarageServiceMock
                 .Verify(m => m.GetByAddress(
                     garageAddress.Country,
                     garageAddress.Province,
                     garageAddress.Locality,
                     garageAddress.District));

            Suite.DomainCompanyServiceMock
                 .Verify(m => m.Create(dispatcherCompanyModel.Dispatcher.PhoneNumber));

            Suite.DomainDispatcherServiceMock
                 .Verify(m => m.Create(
                     dispatcher.FirstName,
                     dispatcher.LastName,
                     dispatcher.PhoneNumber,
                     domainCompany.Id));

            Suite.VehicleServiceMock
                .Verify(m => m.CreateDomainVehicle(
                    domainCompany.Id,
                    It.Is<VehicleAM>(v => v == vehicle)), Times.Once);

            Suite.DomainDriverServiceMock
                 .Verify(m => m.Create(
                     driver.FirstName,
                     driver.LastName,
                     driver.PhoneNumber,
                     domainCompany.Id));

            Suite.DomainDriverServiceMock
                 .Verify(m => m.AssignVehicle(
                     domainDriver.Id,
                     domainVehicle.Id));

            Suite.TransactionMock
                 .Verify(m => m.Commit());

            Suite.TransactionMock
                 .Verify(m => m.Rollback(), Times.Never);
        }
    }
}