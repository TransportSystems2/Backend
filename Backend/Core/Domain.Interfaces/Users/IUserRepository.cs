using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Users
{
    public interface IUserRepository<T> : IRepository<T> where T: User
    {
        Task<T> GetByIndentityUser(int userIdentity);

        Task<bool> IsExistByIdentityUser(int userIdnentity);
    }
}