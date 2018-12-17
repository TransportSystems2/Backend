﻿using Moq;
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
    public class ApplicationCustomerServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationCustomerServiceTestSuite()
        {
            DomainCustomerServiceMock = new Mock<ICustomerService>();

            CustomerService = new ApplicationCustomerService(
                TransactionServiceMock.Object,
                DomainCustomerServiceMock.Object);
        }

        public IApplicationCustomerService CustomerService { get; }

        public Mock<ICustomerService> DomainCustomerServiceMock { get; }
    }

    public class ApplicationCustomerServiceTests : BaseServiceTests<ApplicationCustomerServiceTestSuite>
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
            
            await Suite.CustomerService.GetDomainCustomer(customer);

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

            var result = await Suite.CustomerService.GetDomainCustomer(customer);

            Assert.Equal(domainCustomer.Id, result.Id);
        }
    }
}