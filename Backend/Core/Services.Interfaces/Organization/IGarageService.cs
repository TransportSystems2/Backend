using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Services.Interfaces.Organization
{
    public interface IGarageService : IDomainService<Garage>
    {
        Task<Garage> Create(bool isPublic, int companyId, int addressId, int pricelistId);

        Task AssignPricelist(int garageId, int pricelistId);

        Task<ICollection<string>> GetAvailableProvinces(string country);
        
        Task<ICollection<string>> GetAvailableLocalities(string country, string province);

        Task<ICollection<string>> GetAvailableDistricts(string country, string province, string locality);

        Task<Garage> GetByAddress(string country, string province, string locality, string district);

        Task<Garage> GetByCoordinate(double latitude, double longitude);
    }
}