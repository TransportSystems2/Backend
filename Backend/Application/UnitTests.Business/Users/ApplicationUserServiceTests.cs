using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Business;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Users
{
    public class ApplicationUserServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationUserServiceTestSuite()
        {
            DomainCustomerServiceMock = new Mock<ICustomerService>();
            DomainModeratorServiceMock = new Mock<IModeratorService>();

            Service = new ApplicationUserService(
                TransactionServiceMock.Object,
                DomainCustomerServiceMock.Object,
                DomainModeratorServiceMock.Object);
        }

        public IApplicationUserService Service { get; }

        public Mock<ICustomerService> DomainCustomerServiceMock { get; }

        public Mock<IModeratorService> DomainModeratorServiceMock { get; }
    }

    public class ApplicationUserServiceTests : BaseServiceTests<ApplicationUserServiceTestSuite>
    {
        [Fact]
        public async Task GetDomainCustomerlWhenCustomerDontExist()
        {
            var customer = new CustomerAM
            {
                FirstName = "Гоги",
                LastName = "Вахабитович",
                PhoneNumber = "79998887766"
            };

            Suite.DomainCustomerServiceMock
                .Setup(m => m.GetByPhoneNumber(customer.PhoneNumber))
                .Returns(Task.FromResult<Customer>(null));
            
            await Suite.Service.GetOrCreateDomainCustomer(customer);

            Suite.DomainCustomerServiceMock
                .Verify(m => m.Create(customer.FirstName, customer.LastName, customer.PhoneNumber));
        }

        [Fact]
        public async Task GetDomainCustomerlWhenCustomerExists()
        {
            var customer = new CustomerAM
            {
                FirstName = "Гоги",
                LastName = "Вахабитович",
                PhoneNumber = "79998887766"
            };

            var domainCustomer = new Customer
            {
                Id = 1
            };

            Suite.DomainCustomerServiceMock
                .Setup(m => m.GetByPhoneNumber(customer.PhoneNumber))
                .ReturnsAsync(domainCustomer);

            var result = await Suite.Service.GetOrCreateDomainCustomer(customer);

            Assert.Equal(domainCustomer.Id, result.Id);
        }

        [Fact]
        public async Task GetDomainModeratorByIndetityUser()
        {
            var commonId = 1;
            var identityUserId = commonId++;
            var moderator = new Moderator
            {
                Id = commonId++,
                IdentityUserId = identityUserId
            };

            Suite.DomainModeratorServiceMock
                .Setup(m => m.GetByIndentityUser(identityUserId))
                .ReturnsAsync(moderator);

            var result = await Suite.Service.GetDomainModeratorByIdentityUser(identityUserId);

            Assert.Equal(moderator, result);
        }
    }
}