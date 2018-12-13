using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Routing
{
    public interface IRouteLegRepository : IRepository<RouteLeg>
    {
        Task<ICollection<RouteLeg>> GetByRoute(int routeId, RouteLegKind kind);
    }
}