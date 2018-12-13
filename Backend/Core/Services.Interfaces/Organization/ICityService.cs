using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Services.Interfaces.Organization
{
    public interface ICityService : IDomainService<City>
    {
        Task<City> Create(string domain, int addressId, int pricelistId);

        Task<City> GetByAddress(int addressId);

        Task<bool> IsExistByDomain(string domain);
    }
}