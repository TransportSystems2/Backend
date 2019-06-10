using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Application.Interfaces.Mapping;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationUserService : ApplicationTransactionService, IApplicationUserService
    {
        public ApplicationUserService(
            ITransactionService transactionService,
            IMappingService mappingService,
            ICustomerService domainCustomerService,
            IModeratorService domainModeratorService,
            IDispatcherService domainDispatcherService,
            IIdentityUserService identityUserService)
            : base(transactionService)
        {
            MappingService = mappingService;
            DomainCustomerService = domainCustomerService;
            DomainModeratorService = domainModeratorService;
            DomainDispatcherService = domainDispatcherService;
            IdentityUserService = identityUserService;
        }

        protected IMappingService MappingService { get; }

        protected ICustomerService DomainCustomerService { get; }

        protected IModeratorService DomainModeratorService { get; }

        protected IDispatcherService DomainDispatcherService { get; }

        protected IIdentityUserService IdentityUserService { get; }

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

        public async Task<CustomerAM> GetCustomer(int customerId)
        {
            var domainCustomer = await DomainCustomerService.Get(customerId);
            var identityUser = await IdentityUserService.GetUser(domainCustomer.IdentityUserId);
            var result = new CustomerAM
            {
                Id = customerId,
                FirstName = identityUser.FirstName,
                LastName = identityUser.LastName,
                PhoneNumber = identityUser.PhoneNumber
            };

            return result;
        }
    }
}