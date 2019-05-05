using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Services.Interfaces.Organization
{
    public interface IMarketService : IDomainService<Market>
    {
        Task<Market> Create(int companyId, int addressId, int pricelistId);

        Task<Market> GetByCoordinate(double latitude, double longitude);

        Task<ICollection<Market>> GetByAddressIds(ICollection<int> addressIds);
    }
}