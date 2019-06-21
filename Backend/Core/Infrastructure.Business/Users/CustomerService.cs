using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public class CustomerService : IdentityUserService<Customer>, ICustomerService
    {
        public CustomerService(ICustomerRepository repository)
            : base(repository)
        {
        }

        protected new ICustomerRepository Repository => (ICustomerRepository)base.Repository;

        public override string GetDefaultRole()
        {
            return IdentityUser.CustomerRoleName;
        }
    }
}