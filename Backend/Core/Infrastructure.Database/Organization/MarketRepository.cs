using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Organization
{
    public class MarketRepository : Repository<Market>, IMarketRepository
    {
        public MarketRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<Market> GetByAddress(int addressId)
        {
            return Entities.SingleOrDefaultAsync(e => e.AddressId.Equals(addressId));
        }

        public async Task<ICollection<Market>> GetByAddressIds(ICollection<int> addressIds)
        {
            return await Entities.Where(e => addressIds.Contains(e.AddressId)).ToListAsync();
        }
    }
}