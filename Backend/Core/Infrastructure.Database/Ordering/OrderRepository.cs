using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Ordering
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}