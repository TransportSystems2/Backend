using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Services.Interfaces.Users
{
    public interface IUserService<T> : IDomainService<T> where T : User
    {
        Task<T> Create(string firstName, string lastName, string phoneNumber);

        Task<T> GetByPhoneNumber(string phoneNumber);

        Task<T> GetByIndentityUser(int identityUserId);

        Task<bool> IsExistByIdentityUser(int identityUserId);

        Task<string[]> GetSpecificRoles();
    }
}