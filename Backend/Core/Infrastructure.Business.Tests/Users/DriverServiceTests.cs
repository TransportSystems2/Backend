using System;
using System.Threading.Tasks;
using Moq;
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

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Users
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
        public DriverServiceTests()
        {
            Suite = new DriverServiceTestSuite();
        }

        protected DriverServiceTestSuite Suite { get; }

        [Fact]
        public async Task GetSpecificRoles()
        {
            var specificRoles = await Suite.DriverService.GetSpecificRoles();

            Assert.Contains(UserRole.DriverRoleName, specificRoles);
        }

        [Fact]
        public async Task AssignVehicle()
        {
            var driver = new Driver();
            var vehicle = new Vehicle { Id = 3 };
            var company = new Company();

            Suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);

            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(company.Id))
                .ReturnsAsync(true);

            await Suite.DriverService.AssignVehicle(driver.Id, vehicle.Id);

            Suite.DriverRepositoryMock
                .Verify(m => m.Update(It.Is<Driver>(d => d.VehicleId.Equals(vehicle.Id))));

            Suite.DriverRepositoryMock.Verify(m => m.Save());

            Assert.Equal(driver.VehicleId, vehicle.Id);
        }

        [Fact]
        public async Task AssignVehicleToNotExistDriver()
        {
            var driverId = 0;
            var vehicleId = 1;

            Suite.DriverRepositoryMock
                .Setup(m => m.Get(driverId))
                .Returns(Task.FromResult<Driver>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => Suite.DriverService.AssignVehicle(driverId, vehicleId));
        }

        [Fact]
        public async Task AssignNotExistVehicle()
        {
            var driver = new Driver();
            var vehicleId = 1;

            Suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);

            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicleId))
                .Returns(Task.FromResult<Vehicle>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => Suite.DriverService.AssignVehicle(driver.Id, vehicleId));
        }

        [Fact]
        public async Task AssignVehicleFromAnotherCompany()
        {
            var driver = new Driver { CompanyId = 1 };
            var vehicle = new Vehicle { CompanyId = 2 };

            Suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);

            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);

            await Assert.ThrowsAsync<ArgumentException>(() => Suite.DriverService.AssignVehicle(driver.Id, vehicle.Id));
        }

        [Fact]
        public async Task AssignVehicleToDriverFromNotExistCompany()
        {
            var companyId = 3;

            var driver = new Driver { CompanyId = companyId };
            var vehicle = new Vehicle { CompanyId = companyId };

            Suite.DriverRepositoryMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);

            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => Suite.DriverService.AssignVehicle(driver.Id, vehicle.Id));
        }
    }
}