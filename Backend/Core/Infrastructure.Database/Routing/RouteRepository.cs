using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Routing
{
    public class RouteRepository : Repository<Route>, IRouteRepository
    {
        public RouteRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}