using Microsoft.AspNetCore.Identity;

using System.Linq;
using System.Threading.Tasks;

using TransportSystems.Backend.Identity.Core.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Interfaces
{
    public interface IRoleService
    {
        IQueryable<UserRole> Roles { get; }

        Task<UserRole> FindByIdAsync(string id);

        Task<UserRole> FindByNameAsync(string name);

        Task<IdentityResult> CreateAsync(UserRole role);

        Task<IdentityResult> DeleteAsync(UserRole role);
    }
}
