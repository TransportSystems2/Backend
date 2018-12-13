using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;

namespace TransportSystems.Backend.Core.Infrastructure.Database
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}