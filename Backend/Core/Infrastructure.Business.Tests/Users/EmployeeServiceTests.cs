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
    }
}