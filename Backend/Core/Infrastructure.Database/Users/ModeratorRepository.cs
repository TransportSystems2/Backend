using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Users
{
    public class ModeratorRepository : EmployeeRepository<Moderator>, IModeratorRepository
    {
        public ModeratorRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}