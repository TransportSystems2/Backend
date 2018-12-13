using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Organization
{
    public class CityRepository :
        Repository<City>,
        ICityRepository
    {
        public CityRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<City> GetByAddress(int addressId)
        {
            return Entities.SingleOrDefaultAsync(c => c.AddressId.Equals(addressId));
        }

        public Task<City> GetByDomain(string domain)
        {
            return Entities.SingleOrDefaultAsync(c => c.Domain.Equals(domain));
        }

        public async Task<bool> IsExistByDomain(string domain)
        {
            return await GetByDomain(domain) != null;
        }
    }
}