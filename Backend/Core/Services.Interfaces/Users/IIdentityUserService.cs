using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Services.Interfaces.Users
{
    public interface IIdentityUserService
    {
        Task <IdentityUser> Create(string firstName, string lastName, string phoneNumber);

        Task<bool> IsExistById(int id);

        Task<bool> IsExistByPhoneNumber(string phoneNumber);

        Task<bool> IsInRole(int userId, string role);

        Task<bool> IsUndefined(int userId);

        Task AsignToRoles(int id, string[] roles);

        Task AssignName(int id, string firstName, string lastName);

        Task<ICollection<IdentityUser>> GetUsers();

        Task<IdentityUser> GetUser(int id);

        Task DeleteUser(int id);

        Task<IdentityUser> GetUserByPhoneNumber(string phoneNumber);
    }
}