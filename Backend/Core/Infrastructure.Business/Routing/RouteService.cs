using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Routing
{
    public class RouteService : DomainService<Route>, IRouteService
    {
        public RouteService(
            IRouteRepository repository)
            : base(repository)
        {
        }

        protected new IRouteRepository Repository => (IRouteRepository)base.Repository;

        public async Task<Route> Create(string comment)
        {
            var route = new Route
            {
                Comment = comment
            };

            await Verify(route);

            await Repository.Add(route);
            await Repository.Save();

            return route;
        }

        protected override Task<bool> DoVerifyEntity(Route entity)
        {
            return Task.FromResult(true);
        }
    }
}