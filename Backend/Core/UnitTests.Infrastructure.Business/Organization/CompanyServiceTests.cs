using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Infrastructure.Business.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;

using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Oraganization
{
    public class CompanyServiceTestSuite
    {
        public CompanyServiceTestSuite()
        {
            CompanyRepositoryMock = new Mock<ICompanyRepository>();
            GarageServiceMock = new Mock<IGarageService>();

            CompanyService = new CompanyService(CompanyRepositoryMock.Object, GarageServiceMock.Object);
        }

        public Mock<ICompanyRepository> CompanyRepositoryMock { get; }

        public Mock<IGarageService> GarageServiceMock { get; }

        public ICompanyService CompanyService { get; }
    }

    public class CompanyServiceTest
    {
        [Fact]
        public async void CreateCompanyResultCreatedCompany()
        {
            var suite = new CompanyServiceTestSuite();
            var garage = new Garage { Id = 1 };
            var companyName = "Транспортные Системы";

            suite.GarageServiceMock
                .Setup(m => m.IsExist(garage.Id))
                .ReturnsAsync(true);
            suite.CompanyRepositoryMock
                .Setup(m => m.GetByName(companyName))
                .Returns<Company>(null);

            var resultCompany = await suite.CompanyService.Create(garage.Id, companyName);

            Assert.NotNull(resultCompany);
            Assert.Equal(garage.Id, resultCompany.GarageId);
            Assert.Equal(companyName, resultCompany.Name);
        }
    }
}