using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Routing
{
    public class RouteLegRepository : Repository<RouteLeg>, IRouteLegRepository
    {
        public RouteLegRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<ICollection<RouteLeg>> GetByRoute(int routeId, RouteLegKind kind)
        {
            return await Entities.Where(l => l.RouteId.Equals(routeId) && l.Kind.Equals(kind)).ToListAsync();
        }
    }
}