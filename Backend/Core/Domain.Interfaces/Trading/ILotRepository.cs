using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Trading
{
    public interface ILotRepository : IRepository<Lot>
    {
        Task<Lot> GetByOrder(int orderId);

        Task<ICollection<Lot>> GetByStatus(LotStatus status);
    }
}