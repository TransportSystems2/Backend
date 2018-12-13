using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;

namespace TransportSystems.Backend.Core.Services.Interfaces.Trading
{
    public interface ILotService : IDomainService<Lot>
    {
        Task<Lot> GetByOrder(int orderId);

        Task<ICollection<Lot>> GetByStatus(LotStatus status);

        Task Trade(int lotId);

        Task Win(int lotId, int dispatcherId);

        Task Cancel(int lotId);
    }
}