using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;

namespace TransportSystems.Backend.Core.Services.Interfaces.Interfaces
{
    public interface IOrderService : IDomainService<Order>
    {
        Task<Order> Create();
    }
}