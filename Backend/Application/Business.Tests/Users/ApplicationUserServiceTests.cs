using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Business;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests.Users
{
    public class ApplicationUserServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationUserServiceTestSuite()
        {
            DomainCustomerServiceMock = new Mock<ICustomerService>();
            DomainModeratorServiceMock = new Mock<IModeratorService>();
            DomainDispatcherServiceMock = new Mock<IDispatcherService>();

            Service = new ApplicationUserService(
                TransactionServiceMock.Object,
                MappingService,
                DomainCustomerServiceMock.Object,
                DomainModeratorServiceMock.Object,
                DomainDispatcherServiceMock.Object);
        }

        public IApplicationUserService Service { get; }

        public Mock<ICustomerService> DomainCustomerServiceMock { get; }

        public Mock<IModeratorService> DomainModeratorServiceMock { get; }

        public Mock<IDispatcherService> DomainDispatcherServiceMock { get; }
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
            var moderator = new Moderator
            {
                Id = commonId++,
            };

            Suite.DomainModeratorServiceMock
                .Setup(m => m.Get(moderator.Id))
                .ReturnsAsync(moderator);

            var result = await Suite.Service.GetDomainModerator(moderator.Id);

            Assert.Equal(moderator, result);
        }

        [Fact]
        public async Task GetDomainDispatcherByIndetityUser()
        {
            var commonId = 1;
            var dispatcher = new Dispatcher
            {
                Id = commonId++,
            };

            Suite.DomainDispatcherServiceMock
                .Setup(m => m.Get(dispatcher.Id))
                .ReturnsAsync(dispatcher);

            var result = await Suite.Service.GetDomainDispatcher(dispatcher.Id);

            Assert.Equal(dispatcher, result);
        }

        [Fact]
        public async Task GetDomainDispatcher()
        {
            var commonId = 1;
            var dispatcher = new Dispatcher { Id = commonId++ };

            Suite.DomainDispatcherServiceMock
                .Setup(m => m.Get(dispatcher.Id))
                .ReturnsAsync(dispatcher);

            var result = await Suite.Service.GetDomainDispatcher(dispatcher.Id);

            Assert.Equal(dispatcher, result);
        }
    }
}