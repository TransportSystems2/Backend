using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Business.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Users
{
    public class EmployeeServiceTestSuite
    {
        public EmployeeServiceTestSuite()
        {
            CompanyServiceMock = new Mock<ICompanyService>();

            EmployeeRepositoryMock = new Mock<IEmployeeRepository<Dispatcher>>();

            ServiceMock = new Mock<EmployeeService<Dispatcher>>(
                EmployeeRepositoryMock.Object,
                CompanyServiceMock.Object);
            ServiceMock.CallBase = true;
        }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<IEmployeeRepository<Dispatcher>> EmployeeRepositoryMock { get; }

        public Mock<EmployeeService<Dispatcher>> ServiceMock { get; }

        public EmployeeService<Dispatcher> Service => ServiceMock.Object;
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
            var commonId = 1;
            var companyId = commonId++;
            var firstName = "Alexandr";
            var lastName = "Fadeev";
            var phoneNumber = "79998887766";

            var identityUser = new Dispatcher
            {
                Id = commonId++,
                CompanyId = companyId
            };

            Suite.CompanyServiceMock
                .Setup(m => m.IsExist(companyId))
                .ReturnsAsync(true);

            Suite.EmployeeRepositoryMock
                .Setup(m => m.IsExistByPhoneNumber(phoneNumber))
                .ReturnsAsync(false);

            var result = await Suite.Service.Create(firstName, lastName, phoneNumber, companyId);

            Suite.EmployeeRepositoryMock
                .Verify(m => m.Update(
                    It.Is<Dispatcher>(e => e.CompanyId.Equals(companyId))));

            Suite.EmployeeRepositoryMock
                .Verify(m => m.Save());
        }
    }
}