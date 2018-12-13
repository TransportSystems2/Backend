using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Transport;

namespace TransportSystems.Backend.Core.Services.Interfaces.Transport
{
    public interface ICargoService : IDomainService<Cargo>
    {
        Task<Cargo> Create(
            int weightCatalogItemId,
            int kindCatalogItemId,
            int brandCatalogItemId,
            string registrationNumber = null,
            string comment = null);
    }
}