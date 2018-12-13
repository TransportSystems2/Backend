using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface IApplicationVehicleService : IApplicationTransactionService
    {
        Task<Vehicle> CreateDomainVehicle(int companyId, VehicleAM vehicle);

        Task<VehicleCatalogItemsAM> GetCatalogItems();
    }
}