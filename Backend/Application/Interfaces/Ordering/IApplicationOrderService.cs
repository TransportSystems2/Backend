using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Ordering;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface IApplicationOrderService : IApplicationTransactionService
    {
        Task<ICollection<OrderGroupAM>> GetOrderGroupsByStatuses(OrderStatus[] statuses);

        Task<ICollection<OrderInfoAM>> GetOrdersByStatus(OrderStatus status);

        Task<ICollection<Order>> GetDomainOrdersByStatus(OrderStatus status);

        Task<Order> CreateOrder(BookingAM booking);

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
    }
}