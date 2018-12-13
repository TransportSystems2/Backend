using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Users
{
    public class DispatcherRepository : EmployeeRepository<Dispatcher>, IDispatcherRepository
    {
        public DispatcherRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}