using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Business.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Users
{
    public class DispatcherServiceTestSuite
    {
        public DispatcherServiceTestSuite()
        {
            DispatcherRepositoryMock = new Mock<IDispatcherRepository>();
            CompanyServiceMock = new Mock<ICompanyService>();

            DispatcherService = new DispatcherService(DispatcherRepositoryMock.Object, CompanyServiceMock.Object);
        }

        public Mock<IDispatcherRepository> DispatcherRepositoryMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public IDispatcherService DispatcherService { get; }
    }

    public class DispatcherServiceTests
    {
        public DispatcherServiceTests()
        {
            Suite = new DispatcherServiceTestSuite();
        }

        protected DispatcherServiceTestSuite Suite { get; }

        [Fact]
        public void GetSpecificRoles()
        {
            var roles = Suite.DispatcherService.GetSpecificRoles();

            Assert.Contains(IdentityUser.DispatcherRoleName, roles);
        }
    }
}