using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Users
{
    public class EmployeeRepository<T> :
        UserRepository<T>,
        IEmployeeRepository<T>
        where T : Employee
    {
        public EmployeeRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<ICollection<T>> GetByCompany(int companyId)
        {
            return await Entities.Where(d => d.CompanyId.Equals(companyId)).ToListAsync();
        }
    }
}