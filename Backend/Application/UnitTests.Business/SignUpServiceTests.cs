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
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business
{
    public class SignUpServiceTestsSuite : TransactionTestsSuite
    {
        public SignUpServiceTestsSuite()
        {
            DomainDriverServiceMock = new Mock<IDriverService>();
            DomainDispatcherServiceMock = new Mock<IDispatcherService>();
            DomainCompanyServiceMock = new Mock<ICompanyService>();
            DomainGarageServiceMock = new Mock<IGarageService>();
            VehicleServiceMock = new Mock<IApplicationVehicleService>();

            SignUpService = new SignUpService(
                TransactionServiceMock.Object,
                DomainDriverServiceMock.Object,
                DomainDispatcherServiceMock.Object,
                DomainCompanyServiceMock.Object,
                DomainGarageServiceMock.Object,
                VehicleServiceMock.Object);
        }

        public ISignUpService SignUpService { get; }

        public Mock<IDriverService> DomainDriverServiceMock { get; }

        public Mock<IDispatcherService> DomainDispatcherServiceMock { get; }

        public Mock<ICompanyService> DomainCompanyServiceMock { get; }

        public Mock<IGarageService> DomainGarageServiceMock { get; }

        public Mock<IApplicationVehicleService> VehicleServiceMock { get; }
    }

    public class SignUpServiceTests : BaseServiceTests<SignUpServiceTestsSuite>
    {
        [Fact]
        public async void SignUpDriverCompany()
        {
            var ApplicationVehicle = new VehicleAM
            {
                RegistrationNumber = "К100ЕЕ77",
                BrandCatalogItemId = 0,
                CapacityCatalogItemId = 1,
                KindCatalogItemId = 2
            };

            var ApplicationDriver = new DriverAM
            {
                IdentityUserId = 6,
                FirstName = "Петя",
                LastName = "Лиссерман"
            };

            var garageAddress = new AddressAM
            {
                Country = "Россия",
                Province = "Ярославская",
                Locality = "Рыбинск",
                District = "Центральный"
            };

            var driverCompanyModel = new DriverCompanyAM
            {
                GarageAddress = garageAddress,
                CompanyName = "Транспортные Системы",
                Vehicle = ApplicationVehicle,
                Driver = ApplicationDriver
            };

            var domainCompany = new Company { Id = 1 };
            var domainVehicle = new Vehicle { Id = 2 };
            var domainDriver = new Driver { Id = 3 };
            var domainGarage = new Garage { Id = 4 };

            Suite.DomainGarageServiceMock
                 .Setup(m => m.GetByAddress(garageAddress.Country, garageAddress.Province, garageAddress.Locality, garageAddress.District))
                 .ReturnsAsync(domainGarage);

            Suite.DomainCompanyServiceMock
                 .Setup(m => m.Create(domainGarage.Id, driverCompanyModel.CompanyName))
                 .ReturnsAsync(domainCompany);

            Suite.VehicleServiceMock
                 .Setup(m => m.CreateDomainVehicle(domainCompany.Id, ApplicationVehicle))
                 .ReturnsAsync(domainVehicle);

            Suite.DomainDriverServiceMock
                 .Setup(m => m.Create(ApplicationDriver.FirstName, ApplicationDriver.LastName, ApplicationDriver.PhoneNumber, domainCompany.Id))
                 .ReturnsAsync(domainDriver);

            Suite.TransactionServiceMock
                 .Setup(m => m.BeginTransaction())
                 .ReturnsAsync(Suite.TransactionMock.Object);

            await Suite.SignUpService.SignUpDriverCompany(driverCompanyModel);

            Suite.DomainGarageServiceMock
                 .Verify(m => m.GetByAddress(garageAddress.Country, garageAddress.Province, garageAddress.Locality, garageAddress.District), Times.Once);

            Suite.DomainCompanyServiceMock
                 .Verify(m => m.Create(domainGarage.Id, driverCompanyModel.CompanyName), Times.Once);

            Suite.VehicleServiceMock
                 .Verify(m => m.CreateDomainVehicle(
                     domainCompany.Id,
                     It.Is<VehicleAM>(v => v == ApplicationVehicle)), Times.Once);
            
            Suite.DomainDispatcherServiceMock
                 .Verify(m => m.Create(ApplicationDriver.FirstName, ApplicationDriver.LastName, ApplicationDriver.PhoneNumber, domainCompany.Id), Times.Once);

            Suite.DomainDriverServiceMock
                 .Verify(m => m.Create(ApplicationDriver.FirstName, ApplicationDriver.LastName, ApplicationDriver.PhoneNumber, domainCompany.Id), Times.Once);

            Suite.DomainDriverServiceMock
                 .Verify(m => m.AssignVehicle(domainDriver.Id, domainVehicle.Id), Times.Once);

            Suite.TransactionMock
                 .Verify(m => m.Commit(), Times.Once);

            Suite.TransactionMock
                 .Verify(m => m.Rollback(), Times.Never);
        }

        [Fact]
        public async Task SignUpDispatcherDriver()
        {
            var ApplicationDispatcher = new DispatcherAM
            {
                IdentityUserId = 6,
                FirstName = "Петя",
                LastName = "Лиссерман"
            };

            var garageAddress = new AddressAM
            {
                Country = "Россия",
                Province = "Ярославская",
                Locality = "Рыбинск",
                District = "Центральный"
            };

            var dispatcherCompanyModel = new DispatcherCompanyAM
            {
                GarageAddress = garageAddress,
                CompanyName = "Транспортные Системы",
                Dispatcher = ApplicationDispatcher
            };

            var domainCompany = new Company { Id = 1 };
            var domainDispatcher = new Dispatcher { Id = 3 };
            var domainGarage = new Garage { Id = 4 };

            Suite.DomainGarageServiceMock
                 .Setup(m => m.GetByAddress(garageAddress.Country, garageAddress.Province, garageAddress.Locality, garageAddress.District))
                 .ReturnsAsync(domainGarage);

            Suite.DomainCompanyServiceMock
                 .Setup(m => m.Create(domainGarage.Id, dispatcherCompanyModel.CompanyName))
                 .ReturnsAsync(domainCompany);

            Suite.DomainDispatcherServiceMock
                 .Setup(m => m.Create(ApplicationDispatcher.FirstName, ApplicationDispatcher.LastName, ApplicationDispatcher.PhoneNumber, domainCompany.Id))
                 .ReturnsAsync(domainDispatcher);

            Suite.TransactionServiceMock
                 .Setup(m => m.BeginTransaction())
                 .ReturnsAsync(Suite.TransactionMock.Object);

            await Suite.SignUpService.SignUpDispatcherCompany(dispatcherCompanyModel);

            Suite.DomainGarageServiceMock
                 .Verify(m => m.GetByAddress(garageAddress.Country, garageAddress.Province, garageAddress.Locality, garageAddress.District), Times.Once);

            Suite.DomainCompanyServiceMock
                 .Verify(m => m.Create(domainGarage.Id, dispatcherCompanyModel.CompanyName), Times.Once);

            Suite.DomainDispatcherServiceMock
                 .Verify(m => m.Create(ApplicationDispatcher.FirstName, ApplicationDispatcher.LastName, ApplicationDispatcher.PhoneNumber, domainCompany.Id), Times.Once);

            Suite.TransactionMock
                 .Verify(m => m.Commit(), Times.Once);

            Suite.TransactionMock
                 .Verify(m => m.Rollback(), Times.Never);
        }
    }
}