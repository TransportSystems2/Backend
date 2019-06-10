using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Ordering;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface IApplicationOrderService : IApplicationTransactionService
    {
        Task<ICollection<OrderGroupAM>> GetGroupsByStatuses(OrderStatus[] statuses);

        Task<ICollection<OrderInfoAM>> GetGroupByStatus(OrderStatus status);

        Task<ICollection<Order>> GetDomainOrdersByStatus(OrderStatus status);

        Task<OrderInfoAM> GetInfo(int orderId);
        
        Task<OrderInfoAM> GetInfo(OrderState orderState);

        Task<DetailOrderInfoAM> GetDetailInfo(int orderId);

        Task<Order> CreateOrder(BookingAM booking, int genDispatcherId);

        Task Accept(int orderId, int genDispatcherId);

        Task ReadyToTrade(int orderId, int genDispatcherId);

        Task Trade(int orderId);

        Task AssignToSubDispatcher(int orderId, int subDispatcherId);

        Task AssignToDriver(int orderId, int subDispatcherId, int driverId);

        Task ConfirmByDriver(int orderId, int driverId);

        Task GoToCustomer(int orderId, int driverId);

        Task ArriveAtLoadingPlace(int orderId, int driverId);

        Task LoadTheVehicle(int orderId, int driverId);

        Task DeliverTheVehicle(int orderId, int driverId);

        Task ReceivePayment(int orderId, int driverId);

        Task Complete(int orderId, int driverId);

        Task Cancel(int orderId, int driverId);
    }
}