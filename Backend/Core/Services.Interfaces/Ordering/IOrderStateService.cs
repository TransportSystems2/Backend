using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;

namespace TransportSystems.Backend.Core.Services.Interfaces.Ordering
{
    public interface IOrderStateService : IDomainService<OrderState>
    {
        Task New(
            int orderId,
            int marketId,
            DateTime timeOfDelivery,
            int customerId,
            int cargoId,
            int routeId,
            int billId);

        Task Accept(int orderId, int genDispatcher);

        Task ReadyToTrade(int orderId, int genDispatcher);

        Task Trade(int orderId);

        Task AssignToSubDispatcher(int orderId, int dispatcherId);

        Task AssignToDriver(int orderId, int dispatcherId, int driverId, int vehicleId);

        Task ConfirmByDriver(int orderId, int driverId);

        Task GoToCustomer(int orderId, int driverId);

        Task ArriveAtLoadingPlace(int orderId, int driverId);

        Task LoadTheVehicle(int orderId, int driverId);

        Task DeliverTheVehicle(int orderId, int driverId);

        Task ReceivePayment(int orderId, int driverId);

        Task Complete(int orderId, int driverId);

        Task Cancel(int orderId, int driverId);

        Task<OrderState> GetCurrentState(int orderId);

        Task<ICollection<OrderState>> GetByCurrentStatus(OrderStatus status);

        Task<int> GetCountByCurrentStatus(OrderStatus status);
    }
}