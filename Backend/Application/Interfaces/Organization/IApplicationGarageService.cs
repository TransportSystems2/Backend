using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Application.Models.Geo;
using Common.Models.Units;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationGarageService : IApplicationTransactionService
    {
        Task<Garage> CreateDomainGarage(int companyId, AddressAM address);

        Task<Garage> GetDomainGarage(int garageId);

        Task<Garage> GetDomainGarageByAddress(AddressAM address);

        Task<Garage> GetDomainGarageByCoordinate(Coordinate coordinate);
    }
}