using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Core.Services.Interfaces.Organization
{
    public interface IGarageService : IDomainService<Garage>
    {
        Task<Garage> Create(int companyId, int addressId);

        Task<Garage> GetByAddress(string country, string province, string locality, string district);

        Task<Garage> GetByCoordinate(double latitude, double longitude);
    }
}