using Moq;

using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Business.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Users
{
    public class DriverServiceTestSuite
    {
        public DriverServiceTestSuite()
        {
            DriverRepositoryMock = new Mock<IDriverRepository>();
            IdentityUserServiceMock = new Mock<IIdentityUserService>();
            CompanyServiceMock = new Mock<ICompanyService>();
            VehicleServiceMock = new Mock<IVehicleService>();

            DriverService = new DriverService(DriverRepositoryMock.Object, IdentityUserServiceMock.Object, CompanyServiceMock.Object, VehicleServiceMock.Object);
        }

        public Mock<IDriverRepository> DriverRepositoryMock { get; }

        public Mock<IIdentityUserService> IdentityUserServiceMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<IVehicleService> VehicleServiceMock { get; }

        public IDriverService DriverService { get; }
    }

    public class DriverServiceTests
    {
        [Fact]
        public async void CreateDriverWithNewUserResultCreatedDriver()
        {
            var suite = new DriverServiceTestSuite();

            var company = new Company { Id = 1, GarageId = 2 };
            var identityUser = new IdentityUser { Id = 3 };
            var userInfo = new { PhoneNumber = "79159882658", FirstName = "Павел", LastName = "Федоров" };

            suite.IdentityUserServiceMock
                 .Setup(m => m.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.IdentityUserServiceMock
                 .Setup(m => m.GetUserByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.IdentityUserServiceMock
                 .Setup(m => m.IsUndefined(identityUser.Id))
                 .ReturnsAsync(true);

            suite.IdentityUserServiceMock
                .Setup(m => m.GetUser(identityUser.Id))
                .ReturnsAsync(identityUser);

            suite.CompanyServiceMock
                .Setup(m => m.IsExist(company.Id))
                .ReturnsAsync(true);

            var Driver = await suite.DriverService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id);

            suite.IdentityUserServiceMock
                .Verify(m => m.AssignName(identityUser.Id, userInfo.FirstName, userInfo.LastName), Times.Once);
            suite.IdentityUserServiceMock
                 .Verify(m => m.AsignToRoles(identityUser.Id, new[] { UserRole.DriverRoleName }), Times.Once);

            suite.DriverRepositoryMock
                 .Verify(m => m.Add(Driver), Times.Once);
            suite.DriverRepositoryMock
                 .Verify(m => m.Save(), Times.Once);

            Assert.Equal(company.Id, Driver.CompanyId);
            Assert.Equal(identityUser.Id, Driver.IdentityUserId);
        }

        [Fact]
        public async void CreateDriverWithExistIdentityUserResultCreatedDriver()
        {
            var suite = new DriverServiceTestSuite();

            var company = new Company();
            var identityUser = new IdentityUser();
            var userInfo = new { PhoneNumber = "79159882658", FirstName = "Павел", LastName = "Федоров" };

            suite.IdentityUserServiceMock
                 .Setup(m => m.IsExistByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(true);

            suite.IdentityUserServiceMock
                 .Setup(m => m.GetUserByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.IdentityUserServiceMock
                 .Setup(m => m.IsUndefined(identityUser.Id))
                 .ReturnsAsync(true);

            suite.CompanyServiceMock
                .Setup(m => m.IsExist(company.Id))
                .ReturnsAsync(true);

            var Driver = await suite.DriverService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id);

            suite.IdentityUserServiceMock
                 .Verify(m => m.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber), Times.Never);
        }

        [Fact]
        public async void CreateDublicateDriverResultEntityAlreadyExistsException()
        {
            var suite = new DriverServiceTestSuite();

            var company = new Company();
            var identityUser = new IdentityUser();
            var userInfo = new { PhoneNumber = "79159882658", FirstName = "Павел", LastName = "Федоров" };

            suite.IdentityUserServiceMock
                 .Setup(m => m.GetUserByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.DriverRepositoryMock
                 .Setup(m => m.GetByIndentityUser(identityUser.Id))
                 .ReturnsAsync(new Driver());

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => suite.DriverService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id));
        }

        [Fact]
        public async void CreateDriverWitNotUndefinedIdentityUserResultArgumentException()
        {
            var suite = new DriverServiceTestSuite();

            var company = new Company();
            var identityUser = new IdentityUser();
            var userInfo = new { PhoneNumber = "79159882658", FirstName = "Павел", LastName = "Федоров" };

            suite.IdentityUserServiceMock
                 .Setup(m => m.GetUserByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.IdentityUserServiceMock
                 .Setup(m => m.IsUndefined(identityUser.Id))
                 .ReturnsAsync(false);

            suite.IdentityUserServiceMock
                 .Setup(m => m.IsInRole(identityUser.Id, UserRole.DispatcherRoleName))
                 .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(() => suite.DriverService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id));
        }

        [Fact]
        public async void CreateDriverWithNotExistCompanyResultEntityNotFoundException()
        {
            var suite = new DriverServiceTestSuite();

            var company = new Company();
            var identityUser = new IdentityUser();
            var userInfo = new { PhoneNumber = "79159882658", FirstName = "Павел", LastName = "Федоров" };

            suite.IdentityUserServiceMock
                 .Setup(m => m.GetUserByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.IdentityUserServiceMock
                 .Setup(m => m.IsUndefined(identityUser.Id))
                 .ReturnsAsync(true);

            suite.CompanyServiceMock
                .Setup(m => m.IsExist(company.Id))
                 .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Company", () => suite.DriverService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id));
        }

        [Fact]
        public async void AssignVehicleResultAssignedVehicle()
        {
            var suite = new DriverServiceTestSuite();

            var company = new Company { Id = 3 };
            var driver = new Driver { Id = 1, CompanyId = company.Id };
            var vehicle = new Vehicle { Id = 2, CompanyId = company.Id };

            suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                 .ReturnsAsync(driver);

            suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                 .ReturnsAsync(vehicle);

            suite.CompanyServiceMock
                .Setup(m => m.IsExist(company.Id))
                .ReturnsAsync(true);

            await suite.DriverService.AssignVehicle(driver.Id, vehicle.Id);

            Assert.Equal(vehicle.Id, driver.VehicleId);
        }

        [Fact]
        public async void AssignVehicleWithNonExistenCompanyResultCompanyNotFoundException()
        {
            var suite = new DriverServiceTestSuite();

            var driver = new Driver { Id = 1, CompanyId = 1 };
            var vehicle = new Vehicle { Id = 2, CompanyId = 1 };

            suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                 .ReturnsAsync(driver);

            suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                 .ReturnsAsync(vehicle);

            suite.CompanyServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Company", () => suite.DriverService.AssignVehicle(driver.Id, vehicle.Id));
        }

        [Fact]
        public async void AssignVehicleWithNonExistentDriverResultDriverNotFoundException()
        {
            var suite = new DriverServiceTestSuite();

            suite.DriverRepositoryMock
                .Setup(m => m.Get(It.IsAny<int>()))
                .Returns(Task.FromResult<Driver>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.DriverService.AssignVehicle(0, 0));
        }

        [Fact]
        public async void AssignVehicleWithNonExistentVehicleResultVehicleNotFoundException()
        {
            var suite = new DriverServiceTestSuite();

            var driver = new Driver { Id = 1 };

            suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                 .ReturnsAsync(driver);

            suite.VehicleServiceMock
                .Setup(m => m.Get(It.IsAny<int>()))
                .Returns(Task.FromResult<Vehicle>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>("Vehicle", () => suite.DriverService.AssignVehicle(driver.Id, 0));
        }

        [Fact]
        public async void AssignVehicleWithDifferentCompanyResultCompanyArgumentException()
        {
            var suite = new DriverServiceTestSuite();

            var oneCompany = new Company { Id = 3 };
            var otherCompany = new Company { Id = 4 };

            var driver = new Driver { Id = 1, CompanyId = oneCompany.Id };
            var vehicle = new Vehicle { Id = 2, CompanyId = otherCompany.Id };

            suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                 .ReturnsAsync(driver);

            suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                 .ReturnsAsync(vehicle);

            await Assert.ThrowsAsync<ArgumentException>("Company", () => suite.DriverService.AssignVehicle(driver.Id, vehicle.Id));
        }
    }
}