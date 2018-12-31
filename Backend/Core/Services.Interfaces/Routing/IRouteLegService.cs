using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;

namespace TransportSystems.Backend.Core.Services.Interfaces.Routing
{
    public interface IRouteLegService : IDomainService<RouteLeg>
    {
        Task<RouteLeg> Create(
            int routeId,
            RouteLegKind kind,
            int startAddressId,
            int endAddressId,
            TimeSpan duration,
            int distance);

        Task<ICollection<RouteLeg>> GetByRoute(int routeId, RouteLegKind kind);
    }
}