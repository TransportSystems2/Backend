using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Geo
{
    public class OrderStateRepository : Repository<OrderState>, IOrderStateRepository
    {
        public OrderStateRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<OrderState> GetCurrentState(int orderId)
        {
            return Entities
                .Where(s => s.OrderId.Equals(orderId))
                .OrderByDescending(s => s.Id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<OrderState>> GetStatesByCurrentStatus(OrderStatus status)
        {
            return await Entities
                .GroupBy(s => s.OrderId)
                .Select(g => g.OrderByDescending(s => s.Id).FirstOrDefault())
                .Where(s => s.Status.Equals(status)).ToListAsync();
        }

        public async Task<int> GetCountStatesByCurrentStatus(OrderStatus status)
        {
            return await Entities
                .GroupBy(s => s.OrderId)
                .Select(g => g.OrderByDescending(s => s.Id).FirstOrDefault())
                .Where(s => s.Status.Equals(status)).CountAsync();
        }
    }
}