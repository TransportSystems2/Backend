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
            IDriverService domainDriverService)
            : base(transactionService)
        {
            MappingService = mappingService;
            DomainCustomerService = domainCustomerService;
            DomainModeratorService = domainModeratorService;
            DomainDispatcherService = domainDispatcherService;
            DomainDriverService = domainDriverService;
        }

        protected IMappingService MappingService { get; }

        protected ICustomerService DomainCustomerService { get; }

        protected IModeratorService DomainModeratorService { get; }

        protected IDispatcherService DomainDispatcherService { get; }
        
        protected IDriverService DomainDriverService { get; }

        public async Task<Customer> GetOrCreateDomainCustomer(CustomerAM customer)
        {
            var result = await CreateDomainUser(DomainCustomerService, customer);

            return result;
        }

        public Task<Moderator> GetDomainModerator(int moderatorId)
        {
            return DomainModeratorService.Get(moderatorId);
        }

        public Task<Dispatcher> GetDomainDispatcher(int dispatcherId)
        {
            return DomainDispatcherService.Get(dispatcherId);
        }

        public async Task<CustomerAM> GetCustomer(int customerId)
        {
            var domainCustomer = await DomainCustomerService.Get(customerId);

            return MappingService.Map<CustomerAM>(domainCustomer);
        }

        public Task<Dispatcher> CreateDomainDispatcher(DispatcherAM dispatcher, int companyId)
        {
            return CreateDomainEmployee(DomainDispatcherService, dispatcher, companyId);
        }

        public Task<Driver> CreateDomainDriver(DriverAM driver, int companyId)
        {
            return CreateDomainEmployee(DomainDriverService, driver, companyId);;
        }

        protected async Task<TUser> CreateDomainUser<TUser>(
            IIdentityUserService<TUser> domainUserService,
            UserAM user)
            where TUser : IdentityUser
        {
            var domainUser = await domainUserService.GetByPhoneNumber(user.PhoneNumber);

            if (domainUser == null)
            {
                domainUser = await domainUserService.Create(user.PhoneNumber); 
            }

            if (await domainUserService.IsNeedAssignName(domainUser.Id))
            {
                domainUser = await domainUserService.AsignName(domainUser.Id, user.FirstName, user.LastName);
            }

            return domainUser;
        }

        protected async Task<TEmployee> CreateDomainEmployee<TEmployee>(
            IEmployeeService<TEmployee> domainEmployeeService,
            UserAM employee,
            int companyId)
            where TEmployee : Employee
        {
            var domainEmployee = await CreateDomainUser(domainEmployeeService, employee);

            if (!domainEmployee.CompanyId.Equals(companyId))
            {
                domainEmployee = await domainEmployeeService.AssignCompany(domainEmployee.Id, companyId);
            }

            return domainEmployee;
        }
    }
}