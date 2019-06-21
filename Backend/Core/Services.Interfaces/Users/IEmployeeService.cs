using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Services.Interfaces.Users
{
    public interface IEmployeeService<T> : IIdentityUserService<T> where T : Employee
    {
        Task<T> AssignCompany(int id, int companyId);

        Task<ICollection<T>> GetByCompany(int companyId, string role);
    }
}
