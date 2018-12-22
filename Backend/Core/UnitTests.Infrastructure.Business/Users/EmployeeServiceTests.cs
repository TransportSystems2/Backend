using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Business.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Users
{
    public class TestEmployee : Employee
    {
    }

    public class TestEmployeeService<T> : EmployeeService<T>, IUserService<T> where T : TestEmployee, new()
    {
        public TestEmployeeService(
            IEmployeeRepository<T> repository,
            IIdentityUserService identityUserService,
            ICompanyService companyService)
            : base(repository, identityUserService, companyService)
        {
        }

        public override Task<string[]> GetSpecificRoles()
        {
            var specificRoles = new string[] { UserRole.EmployeeRoleName };

            return Task.FromResult(specificRoles);
        }
    }
    public class EmployeeServiceTestSuite
    {
        public EmployeeServiceTestSuite()
        {
            IdentityUserServiceMock = new Mock<IIdentityUserService>();
            CompanyServiceMock = new Mock<ICompanyService>();

            EmployeeRepositoryMock = new Mock<IEmployeeRepository<TestEmployee>>();

            EmployeeService = new TestEmployeeService<TestEmployee>(EmployeeRepositoryMock.Object, IdentityUserServiceMock.Object, CompanyServiceMock.Object);
        }

        public Mock<IIdentityUserService> IdentityUserServiceMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<IEmployeeRepository<TestEmployee>> EmployeeRepositoryMock { get; }

        public IEmployeeService<TestEmployee> EmployeeService { get; }
    }

    public class EmployeeServiceTests
    {
        public EmployeeServiceTests()
        {
            Suite = new EmployeeServiceTestSuite();
        }

        protected EmployeeServiceTestSuite Suite { get; }

        [Fact]
        public async Task Create()
        {
            var firstName = "Alexandr";
            var lastName = "Fadeev";
            var phoneNumber = "+77777777";
            var companyId = 1;

            var identityUserId = 1;
            var identityUser = new IdentityUser { Id = identityUserId };

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);

            Suite.IdentityUserServiceMock
                .Setup(m => m.GetUserByPhoneNumber(phoneNumber))
                .ReturnsAsync(identityUser);

            Suite.IdentityUserServiceMock
                .Setup(m => m.IsExistById(identityUserId))
                .ReturnsAsync(true);

            var result = await Suite.EmployeeService.Create(firstName, lastName, phoneNumber, companyId);

            Suite.EmployeeRepositoryMock
                .Verify(m => m.Update(It.Is<TestEmployee>(e => e.CompanyId.Equals(companyId))));

            Suite.EmployeeRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(companyId, result.CompanyId);
        }

        [Fact]
        public async Task GetSpecificRoles()
        {
            var roles = await Suite.EmployeeService.GetSpecificRoles();

            Assert.Contains(UserRole.EmployeeRoleName, roles);
        }

    }
}
