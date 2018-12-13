using TransportSystems.Backend.Core.Domain.Core.Routing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Routing
{
    public class PathPointRepository : Repository<TrackPoint>
    {
        public PathPointRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}