using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.SignUp;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using Xunit;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Users;

namespace TransportSystems.Backend.Application.Business.Tests
{
    public class SignUpServiceTestsSuite : TransactionTestsSuite
    {
        public SignUpServiceTestsSuite()
        {
            DomainCompanyServiceMock = new Mock<ICompanyService>();
            GarageServiceMock = new Mock<IApplicationGarageService>();
            VehicleServiceMock = new Mock<IApplicationVehicleService>();
            UserServiceMock = new Mock<IApplicationUserService>();

            SignUpService = new SignUpService(
                TransactionServiceMock.Object,
                DomainCompanyServiceMock.Object,
                UserServiceMock.Object,
                GarageServiceMock.Object,
                VehicleServiceMock.Object);
        }

        public ISignUpService SignUpService { get; }

        public Mock<ICompanyService> DomainCompanyServiceMock { get; }

        public Mock<IApplicationGarageService> GarageServiceMock { get; }

        public Mock<IApplicationVehicleService> VehicleServiceMock { get; }

        public Mock<IApplicationUserService> UserServiceMock { get; }
    }

    public class SignUpServiceTests : BaseServiceTests<SignUpServiceTestsSuite>
    {
        [Fact]
        public async Task SignUpCompany()
        {
            var commonId = 1;

            var dispatcher = new DispatcherAM
            {
                PhoneNumber = "+79998887766"
            };

            var garageAddress = new AddressAM();

            var vehicle = new VehicleAM();

            var driver = new DriverAM
            {
                PhoneNumber = "78887775533"
            };

            var companyApplicationModel = new CompanyApplicationAM
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
                 .Setup(m => m.Create(companyApplicationModel.Dispatcher.PhoneNumber))
                 .ReturnsAsync(domainCompany);

            Suite.UserServiceMock
                 .Setup(m => m.CreateDomainDispatcher(
                     dispatcher,
                     domainCompany.Id))
                 .ReturnsAsync(domainDispatcher);

            Suite.VehicleServiceMock
                 .Setup(m => m.CreateDomainVehicle(
                     domainCompany.Id,
                     vehicle))
                 .ReturnsAsync(domainVehicle);

            Suite.UserServiceMock
                 .Setup(m => m.CreateDomainDriver(
                     driver,
                     domainCompany.Id))
                 .ReturnsAsync(domainDriver);

            Suite.TransactionServiceMock
                 .Setup(m => m.BeginTransaction())
                 .ReturnsAsync(Suite.TransactionMock.Object);

            await Suite.SignUpService.SignUpCompany(companyApplicationModel);

            Suite.GarageServiceMock
                 .Verify(m => m.CreateDomainGarage(
                     domainCompany.Id,
                     garageAddress
                 ));

            Suite.DomainCompanyServiceMock
                 .Verify(m => m.Create(companyApplicationModel.Dispatcher.PhoneNumber));

            Suite.UserServiceMock
                 .Verify(m => m.CreateDomainDispatcher(
                     dispatcher,
                     domainCompany.Id));

            Suite.VehicleServiceMock
                .Verify(m => m.CreateDomainVehicle(
                    domainCompany.Id,
                    It.Is<VehicleAM>(v => v == vehicle)), Times.Once);

            Suite.UserServiceMock
                 .Verify(m => m.CreateDomainDriver(
                     driver,
                     domainCompany.Id));

            Suite.TransactionMock
                 .Verify(m => m.Commit());

            Suite.TransactionMock
                 .Verify(m => m.Rollback(), Times.Never);
        }
    }
}