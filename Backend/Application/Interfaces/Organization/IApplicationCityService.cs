using Common.Models.Geolocation;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationCityService : IApplicationTransactionService
    {
        Task<City> CreateDomainCity(string domain, AddressAM address);

        Task<City> GetDomainCityByCoordinate(Coordinate coordinate);

        Task<bool> IsExistByDomain(string domain);

        Task<City> GetDomainCity(int cityId);
    }
}