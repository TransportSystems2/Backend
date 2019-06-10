using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Services.Interfaces.Users
{
    public interface IEmployeeService<T> : IIdentityUserService<T> where T : Employee
    {
        Task<T> Create(string firstName, string lastName, string phoneNumber, int companyId);

        Task<ICollection<T>> GetByCompany(int companyId, string role);
    }
}
