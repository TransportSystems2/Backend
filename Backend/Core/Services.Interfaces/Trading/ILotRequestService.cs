using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;

namespace TransportSystems.Backend.Core.Services.Interfaces.Trading
{
    public interface ILotRequestService : IDomainService<LotRequest>
    {
        Task<LotRequest> Bet(int lotId, int dispatcherId);

        Task<LotRequest> Cancel(int lotId, int dispatcherId);

        Task<ICollection<LotRequest>> GetDispatcherRequests(int dispatcherId);

        Task<ICollection<LotRequest>> GetLotRequests(int lotId);

        Task<LotRequest> GetCurrentRequest(int lotId, int dispatcherId);

        Task<bool> IsExistBet(int lotId, int dispacherId);
    }
}