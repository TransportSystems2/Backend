using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;

namespace TransportSystems.Backend.Core.Services.Interfaces.Routing
{
    public interface IRouteService : IDomainService<Route>
    {
        Task<Route> Create(string comment = null);
    }
}