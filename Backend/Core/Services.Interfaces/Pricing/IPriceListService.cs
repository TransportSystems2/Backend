using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;

namespace TransportSystems.Backend.Core.Services.Interfaces.Pricing
{
    public interface IPricelistService : IDomainService<Pricelist>
    {
        Task<Pricelist> Create();
    }
}