using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public class CustomerService : UserService<Customer>, ICustomerService
    {
        public CustomerService(
            ICustomerRepository repository,
            IIdentityUserService identityUserService)
            : base(repository, identityUserService)
        {
        }

        protected new ICustomerRepository Repository => (ICustomerRepository)base.Repository;

        public override Task<string[]> GetSpecificRoles()
        {
            var result = new string[] { UserRole.CustomerRoleName };
            
            return Task.FromResult(result);
        }
    }
}