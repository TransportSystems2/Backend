using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;

namespace TransportSystems.Backend.Core.Infrastructure.Database
{
    public class CargoRepository : Repository<Cargo>, ICargoRepository
    {
        public CargoRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}