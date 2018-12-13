using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Users
{
    public class DriverRepository : EmployeeRepository<Driver>, IDriverRepository
    {
        public DriverRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}