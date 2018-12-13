using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationCustomerService : ApplicationTransactionService, IApplicationCustomerService
    {
        public ApplicationCustomerService(
            ITransactionService transactionService,
            ICustomerService domainCustomerService)
            : base(transactionService)
        {
            DomainCustomerService = domainCustomerService;
        }

        protected ICustomerService DomainCustomerService { get; }

        public async Task<Customer> GetDomainCustomer(CustomerAM customer)
        {
            var result = await DomainCustomerService.GetByPhoneNumber(customer.PhoneNumber);
            if (result == null)
            {
                result = await DomainCustomerService.Create(
                    customer.FirstName,
                    customer.LastName,
                    customer.PhoneNumber);
            }

            return result;
        }
    }
}