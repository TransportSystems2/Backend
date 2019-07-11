using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;

namespace TransportSystems.Backend.Core.Infrastructure.Database
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<ICollection<Vehicle>> GetByCompany(int companyId)
        {
            return await Entities.Where(e => e.CompanyId.Equals(companyId)).ToListAsync();
        }
    }
}