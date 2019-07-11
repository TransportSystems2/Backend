using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Transport;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface IApplicationVehicleService : IApplicationTransactionService
    {
        Task<Vehicle> CreateDomainVehicle(int companyId, VehicleAM vehicle);

        Task<VehicleAM> GetVehicle(int id);

        Task<VehicleAM> GetVehicle(Vehicle domainVehicle);

        Task<ICollection<VehicleAM>> GetByCompany(int companyId);

        Task<VehicleCatalogItemsAM> GetCatalogItems();
    }
}