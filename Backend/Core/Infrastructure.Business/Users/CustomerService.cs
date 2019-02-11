using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public class CustomerService : EmployeeService<Customer>, ICustomerService
    {
        public CustomerService(
            ICustomerRepository repository,
            IIdentityUserService identityUserService,
            ICompanyService companyService)
            : base(repository, identityUserService, companyService)
        {
        }

        protected new ICustomerRepository Repository => (ICustomerRepository)base.Repository;

        public override async Task<string[]> GetSpecificRoles()
        {
            var baseSpecificRoles = await base.GetSpecificRoles();
            var specificRoles = new List<string>(baseSpecificRoles)
            {
                UserRole.CustomerRoleName
            };

            return specificRoles.ToArray();
        }
    }
}