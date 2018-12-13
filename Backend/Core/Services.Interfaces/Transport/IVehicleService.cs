using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Transport;

namespace TransportSystems.Backend.Core.Services.Interfaces.Transport
{
    public interface IVehicleService : IDomainService<Vehicle>
    {
        Task<Vehicle> Create(
            int companyId,
            string registrationNumber,
            int brandCatalogItemId,
            int capacityCatalogItemId,
            int kindCatalogItemId);
    }
}