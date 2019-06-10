using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Services.Interfaces.Users
{
    public interface IIdentityUserService<T> : IDomainService<T> where T : IdentityUser
    {
        Task<T> Create(string phoneNumber, string firstName, string lastName);

        Task AsignName(int id, string firstName, string lastName);

        Task<bool> IsExistByPhoneNumber(string phoneNumber);

        Task<bool> IsInRole(int id, string role);

        Task<bool> IsUndefined(int id);

        Task AsignToRoles(int id, string[] roles);

        Task<T> GetByPhoneNumber(string phoneNumber);

         string[] GetSpecificRoles();
    }
}