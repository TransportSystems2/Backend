using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Organization
{
    public interface IMarketRepository : IRepository<Market>
    {
        Task<Market> GetByAddress(int addressId);

        Task<ICollection<Market>> GetByAddressIds(ICollection<int> addressIds);
    }
}