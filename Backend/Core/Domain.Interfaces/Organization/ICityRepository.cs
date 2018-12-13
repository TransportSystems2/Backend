using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Organization
{
    public interface ICityRepository : IRepository<City>
    {
        Task<City> GetByDomain(string domain);

        Task<City> GetByAddress(int addressId);

        Task<bool> IsExistByDomain(string domain);
    }
}