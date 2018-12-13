using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface IApplicationCargoService : IApplicationTransactionService
    {
        Task<Cargo> CreateDomainCargo(CargoAM cargo);

        Task<CargoAM> GetCargo(int cargoId);

        Task<Cargo> GetDomainCargo(int cargoId);

        Task<CargoCatalogItemsAM> GetCatalogItems();

        Task<bool> ValidRegistrationNumber(string registrationNumber);
    }
}