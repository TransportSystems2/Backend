using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Users
{
    public interface IIdentityUserRepository<T> : IRepository<T> where T : IdentityUser
    {
        Task<bool> IsExistByPhoneNumber(string phoneNumber);

        Task<bool> IsInRole(int userId, string role);

        Task<bool> IsUndefined(int userId);

        Task<bool> AsignToRoles(int userId, string[] roles);

        Task<T> GetByPhoneNumber(string phoneNumber);
    }
}