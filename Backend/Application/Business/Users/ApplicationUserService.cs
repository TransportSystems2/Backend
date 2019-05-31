using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationUserService : ApplicationTransactionService, IApplicationUserService
    {
        public ApplicationUserService(
            ITransactionService transactionService,
            ICustomerService domainCustomerService,
            IModeratorService domainModeratorService,
            IDispatcherService domainDispatcherService)
            : base(transactionService)
        {
            DomainCustomerService = domainCustomerService;
            DomainModeratorService = domainModeratorService;
            DomainDispatcherService = domainDispatcherService;
        }

        protected ICustomerService DomainCustomerService { get; }

        protected IModeratorService DomainModeratorService { get; }

        protected IDispatcherService DomainDispatcherService { get; }

        public async Task<Customer> GetOrCreateDomainCustomer(CustomerAM customer)
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

        public Task<Moderator> GetDomainModeratorByIdentityUser(int identityUserId)
        {
            return DomainModeratorService.GetByIndentityUser(identityUserId);
        }

        public Task<Dispatcher> GetDomainDispatcherByIdentityUser(int identityUserId)
        {
            return DomainDispatcherService.GetByIndentityUser(identityUserId);
        }

        public Task<Dispatcher> GetDomainDispatcher(int dispatcherId)
        {
            return DomainDispatcherService.Get(dispatcherId);
        }
    }
}