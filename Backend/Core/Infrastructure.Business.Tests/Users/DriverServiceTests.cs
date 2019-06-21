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
            CompanyServiceMock = new Mock<ICompanyService>();
            VehicleServiceMock = new Mock<IVehicleService>();

            DriverService = new DriverService(DriverRepositoryMock.Object, CompanyServiceMock.Object, VehicleServiceMock.Object);
        }

        public Mock<IDriverRepository> DriverRepositoryMock { get; }

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
        public void GetSpecificRoles()
        {
            var specificRoles = Suite.DriverService.GetDefaultRole();

            Assert.Contains(IdentityUser.DriverRoleName, specificRoles);
        }
    }
}