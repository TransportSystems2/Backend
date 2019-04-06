using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationGarageService : IApplicationTransactionService
    {
        Task<Garage> CreateDomainGarage(bool isPublic, int companyId, int cityId, AddressAM address);

        Task<Garage> GetDomainGarage(int garageId);

        Task<ICollection<string>> GetAvailableProvinces(string country);

        Task<ICollection<string>> GetAvailableLocalities(string country, string province);

        Task<ICollection<string>> GetAvailableDistricts(string country, string province, string locality);
    }
}