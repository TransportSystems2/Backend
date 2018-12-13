using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Trading
{
    public class LotRepository : Repository<Lot>, ILotRepository
    {
        public LotRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<Lot> GetByOrder(int orderId)
        {
            return Entities.SingleOrDefaultAsync(l => l.OrderId.Equals(orderId));
        }

        public async Task<ICollection<Lot>> GetByStatus(LotStatus status)
        {
            return await Entities.Where(l => l.Status.Equals(status)).ToListAsync();
        }
    }
}