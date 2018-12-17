﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;

namespace TransportSystems.Backend.Core.Services.Interfaces.Ordering
{
    public interface IOrderStateService : IDomainService<OrderState>
    {
        Task New(int orderId);

        Task Accept(int orderId, int moderatorId);

        Task ReadyToTrade(int orderId, int moderatorId);

        Task Trade(int orderId);

        Task AssignToDispatcher(int orderId, int dispatcherId);

        Task AssignToDriver(int orderId, int dispatcherId, int driverId);

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