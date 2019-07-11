using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Transport;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Transport
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<ICollection<Vehicle>> GetByCompany(int companyId);
    }
}