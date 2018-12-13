using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Users
{
    public abstract class UserRepository<T> : Repository<T>, IUserRepository<T> where T : User
    {
        public UserRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<T> GetByIndentityUser(int userIdentity)
        {
            return Entities.SingleOrDefaultAsync(e => e.IdentityUserId.Equals(userIdentity));
        }

        public Task<bool> IsExistByIdentityUser(int userIdnentity)
        {
            return Entities.AnyAsync(e => e.IdentityUserId.Equals(userIdnentity));
        }
    }
}
