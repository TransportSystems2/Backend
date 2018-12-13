using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Organization
{
    public interface IGarageRepository : IRepository<Garage>
    {
        Task<Garage> GetByAddress(int addressId);
    }
}