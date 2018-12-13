using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Organization
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<Company> GetByName(string name)
        {
            return Entities.SingleOrDefaultAsync(c => c.Name.Equals(name));
        }
    }
}
