using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Business.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Users
{
    public class DispatcherServiceTestSuite
    {
        public DispatcherServiceTestSuite()
        {
            DispatcherRepositoryMock = new Mock<IDispatcherRepository>();
            CompanyServiceMock = new Mock<ICompanyService>();
            IdentityUserServiceMock = new Mock<IIdentityUserService>();

            DispatcherService = new DispatcherService(DispatcherRepositoryMock.Object, IdentityUserServiceMock.Object, CompanyServiceMock.Object);
        }

        public Mock<IDispatcherRepository> DispatcherRepositoryMock { get; }

        public Mock<ICompanyService> CompanyServiceMock { get; }

        public Mock<IIdentityUserService> IdentityUserServiceMock { get; }

        public IDispatcherService DispatcherService { get; }
    }

    public class DispatcherServiceTests
    {
        [Fact]
        public async void CreateDispatcherWithNewUserResultCreatedDispatcher()
        {
            var suite = new DispatcherServiceTestSuite();

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

            var dispatcher = await suite.DispatcherService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id);

            suite.IdentityUserServiceMock
                .Verify(m => m.AssignName(identityUser.Id, userInfo.FirstName, userInfo.LastName), Times.Once);
            suite.IdentityUserServiceMock
                 .Verify(m => m.AsignToRoles(identityUser.Id, new[] { UserRole.DispatcherRoleName, UserRole.EmployeeRoleName }), Times.Once);

            suite.DispatcherRepositoryMock
                 .Verify(m => m.Add(dispatcher), Times.Once);
            suite.DispatcherRepositoryMock
                 .Verify(m => m.Save(), Times.Once);

            Assert.Equal(company.Id, dispatcher.CompanyId);
            Assert.Equal(identityUser.Id, dispatcher.IdentityUserId);
        }

        [Fact]
        public async void CreateDispatcherWithExistIdentityUserResultCreatedDispatcher()
        {
            var suite = new DispatcherServiceTestSuite();

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

            var dispatcher = await suite.DispatcherService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id);

            suite.IdentityUserServiceMock
                 .Verify(m => m.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber), Times.Never);
        }

        [Fact]
        public async void CreateDublicateDispatcherResultEntityAlreadyExistsException()
        {
            var suite = new DispatcherServiceTestSuite();

            var company = new Company();
            var identityUser = new IdentityUser();
            var userInfo = new { PhoneNumber = "79159882658", FirstName = "Павел", LastName = "Федоров" };

            suite.IdentityUserServiceMock
                 .Setup(m => m.GetUserByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.DispatcherRepositoryMock
                 .Setup(m => m.GetByIndentityUser(identityUser.Id))
                 .ReturnsAsync(new Dispatcher());

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => suite.DispatcherService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id));
        }

        [Fact]
        public async void CreateDispatcherWitNotUndefinedIdentityUserResultArgumentException()
        {
            var suite = new DispatcherServiceTestSuite();

            var company = new Company();
            var identityUser = new IdentityUser();
            var userInfo = new { PhoneNumber = "79159882658", FirstName = "Павел", LastName = "Федоров" };

            suite.IdentityUserServiceMock
                 .Setup(m => m.GetUserByPhoneNumber(userInfo.PhoneNumber))
                 .ReturnsAsync(identityUser);

            suite.IdentityUserServiceMock
                 .Setup(m => m.IsUndefined(identityUser.Id))
                 .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(() => suite.DispatcherService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id));
        }

        [Fact]
        public async void CreateDispathcerWithNotExistCompanyResultEntityNotFoundException()
        {
            var suite = new DispatcherServiceTestSuite();

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

            await Assert.ThrowsAsync<EntityNotFoundException>("Company", () => suite.DispatcherService.Create(userInfo.FirstName, userInfo.LastName, userInfo.PhoneNumber, company.Id));
        }
    }
}