using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Ordering
{
    public interface IOrderStateRepository : IRepository<OrderState>
    {
        Task<OrderState> GetCurrentState(int orderId);

        Task<ICollection<OrderState>> GetStatesByCurrentStatus(OrderStatus status);

        Task<int> GetCountStatesByCurrentStatus(OrderStatus status);
    }
}