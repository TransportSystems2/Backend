using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Users
{
    public class CustomerRepository : UserRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}