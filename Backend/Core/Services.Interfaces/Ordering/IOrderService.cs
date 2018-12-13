using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;

namespace TransportSystems.Backend.Core.Services.Interfaces.Interfaces
{
    public interface IOrderService : IDomainService<Order>
    {
        Task<Order> Create(DateTime time, int customerId, int cargoId, int routeId, int billId);

        Task AssignModerator(int orderId, int moderatorId);

        Task AssignDispatcher(int orderId, int dispatcherId);

        Task AssignDriver(int orderId, int driverId);
    }
}