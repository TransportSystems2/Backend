using TransportSystems.Backend.Core.Domain.Core.Routing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Routing
{
    public class PathRepository : Repository<Track>
    {
        public PathRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}