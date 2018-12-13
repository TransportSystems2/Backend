using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TransportSystems.Backend.Identity.Core.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Interfaces
{
    public interface IUserService
    {
        IQueryable<User> Users { get; }

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> CreateAsync(User user);

        Task<IdentityResult> CreateAsync(User user, string password);

        Task <IList<User>> GetUsersInRoleAsync(string role);

        Task<bool> IsInRoleAsync(int id, string role);

        Task<bool> ExistAsync(int id);

        IEnumerable<User> FindByIds(IEnumerable<int> ids);

        Task<User> FindByIdAsync(string id);

        Task<User> FindByIdAsync(int id);

        Task<User> FindByNameAsync(string name);

        User FindByPhoneNumber(string phoneNumber);

        Task<IdentityResult> UpdateAsync(User user);

        Task<IdentityResult> DeleteAsync(string id);

        Task<IList<string>> GetRolesAsync(User user);

        Task<IdentityResult> AddToRolesAsync(User user, IEnumerable<string> roles);

        Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles);
    }
}
