using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Trading
{
    public interface ILotRequestRepository : IRepository<LotRequest>
    {
        Task<ICollection<LotRequest>> GetByDispatcher(int dispatcherId);

        Task<ICollection<LotRequest>> GetByLot(int lotId);

        Task<LotRequest> GetCurrent(int lotId, int dispatcherId);
    }
}