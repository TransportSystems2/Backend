using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Organization
{
    public class GarageRepository : Repository<Garage>, IGarageRepository
    {
        public GarageRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<Garage> GetByAddress(int addressId)
        {
            return Entities.SingleOrDefaultAsync(e => e.AddressId.Equals(addressId));
        }
    }
}