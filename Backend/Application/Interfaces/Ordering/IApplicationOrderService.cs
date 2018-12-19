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
    }
}