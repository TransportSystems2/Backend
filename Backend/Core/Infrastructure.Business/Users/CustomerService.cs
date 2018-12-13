using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
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

        public override string[] GetSpecificRoles()
        {
            return new[] { UserRole.CustomerRoleName };
        }
    }
}