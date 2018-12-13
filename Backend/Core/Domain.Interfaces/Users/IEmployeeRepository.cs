using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Users
{
    public interface IEmployeeRepository<T> : IUserRepository<T> where T : Employee
    {
        Task<ICollection<T>> GetByCompany(int companyId);
    }
}