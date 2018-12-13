using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Trading
{
    public class LotRequestRepository : Repository<LotRequest>, ILotRequestRepository
    {
        public LotRequestRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<ICollection<LotRequest>> GetByDispatcher(int dispatcherId)
        {
            return await Entities.Where(r => r.DispatcherId.Equals(dispatcherId)).ToListAsync();
        }

        public async Task<ICollection<LotRequest>> GetByLot(int lotId)
        {
            return await Entities.Where(r => r.LotId.Equals(lotId)).ToListAsync();
        }

        public Task<LotRequest> GetCurrent(int lotId, int dispatcherId)
        {
            return Entities.Where(r => r.LotId.Equals(lotId) && r.DispatcherId.Equals(dispatcherId)).OrderByDescending(r => r.Id).FirstOrDefaultAsync();
        }
    }
}