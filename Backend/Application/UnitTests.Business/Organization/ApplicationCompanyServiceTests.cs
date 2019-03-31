using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Organization;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Organization
{
    public class ApplicationCompanyServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationCompanyServiceTestSuite()
        {
            DomainCompanyServiceMock = new Mock<ICompanyService>();

            Service = new ApplicationCompanyService(
                TransactionServiceMock.Object,
                DomainCompanyServiceMock.Object);
        }

        public IApplicationCompanyService Service { get; }

        public Mock<ICompanyService> DomainCompanyServiceMock { get; }
    }

    public class ApplicationCompanyServiceTests : BaseServiceTests<ApplicationCompanyServiceTestSuite>
    {
        [Fact]
        public async Task CreateDomainCompany()
        {
            var domainCompany = new Company
            {
                Name = "ГосЭвакуатор",
                IsPrivate = false
            };

            Suite.DomainCompanyServiceMock
                .Setup(m => m.Create(domainCompany.Name, domainCompany.IsPrivate))
                .ReturnsAsync(domainCompany);

            var result = await Suite.Service.CreateDomainCompany(domainCompany.Name, domainCompany.IsPrivate);

            Assert.Equal(domainCompany.Name, result.Name);
            Assert.Equal(domainCompany.IsPrivate, result.IsPrivate);
        }

        [Fact]
        public async Task GetDomainCompany()
        {
            var domainCompany = new Company
            {
                Name = "ГосЭвакуатор",
                IsPrivate = false
            };

            Suite.DomainCompanyServiceMock
                .Setup(m => m.GetByName(domainCompany.Name))
                .ReturnsAsync(domainCompany);

            var result = await Suite.Service.GetDomainCompany(domainCompany.Name);

            Assert.Equal(domainCompany, result);
        }
    }
}