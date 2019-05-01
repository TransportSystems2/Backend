using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Application.Models.Geo;
using Common.Models.Units;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationGarageService : IApplicationTransactionService
    {
        Task<Garage> CreateDomainGarage(bool isPublic, int companyId, AddressAM address);

        Task<Garage> GetDomainGarage(int garageId);

        Task<Garage> GetDomainGarageByAddress(AddressAM address);

        Task<Garage> GetDomainGarageByCoordinate(Coordinate coordinate);

        Task<ICollection<string>> GetAvailableProvinces(string country);

        Task<ICollection<string>> GetAvailableLocalities(string country, string province);

        Task<ICollection<string>> GetAvailableDistricts(string country, string province, string locality);
    }
}