using DotNetDistance;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;

namespace TransportSystems.Backend.Core.Services.Interfaces.Billing
{
    public interface IBasketService : IDomainService<Basket>
    {
        Task<Basket> Create(
            Distance distance,
            int loadingValue,
            int lockedSteeringValue,
            int lockedWheelsValue,
            int overturnedValue,
            int ditchValue);
    }
}