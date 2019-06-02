using Common.Models.Units;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Routing;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests.Routing
{
    public class RouteLegRepositoryTests : BaseRepositoryTests<IRouteLegRepository, RouteLeg>
    {
        [Fact]
        public async Task GetByRoute()
        {
            var routeId = 15;
            var kind = RouteLegKind.Transportation;

            var items = new[]
            {
                new RouteLeg { Id = 1, RouteId = routeId, Kind = RouteLegKind.Feed },
                new RouteLeg { Id = 3, RouteId = routeId, Kind = kind },
                new RouteLeg { Id = 4, RouteId = routeId, Kind = kind },
                new RouteLeg { Id = 5, RouteId = 5, Kind = kind },
            };

            await Repository.AddRange(items);
            await Repository.Save();

            var result = await Repository.GetByRoute(routeId, kind);

            Assert.Equal(2, result.Count);
            Assert.Equal(3, result.ElementAt(0).Id);
            Assert.Equal(4, result.ElementAt(1).Id);
        }

        [Fact]
        public async Task GetByRouteWhenGetOnlyTransactionAndWayBackKinds()
        {
            var routeId = 15;

            var items = new[]
            {
                new RouteLeg { Id = 1, RouteId = routeId, Kind = RouteLegKind.Feed },
                new RouteLeg { Id = 3, RouteId = routeId, Kind = RouteLegKind.Transportation },
                new RouteLeg { Id = 4, RouteId = routeId, Kind = RouteLegKind.WayBack },
                new RouteLeg { Id = 5, RouteId = 5, Kind = RouteLegKind.Transportation },
            };

            await Repository.AddRange(items);
            await Repository.Save();

            var result = await Repository.GetByRoute(routeId, RouteLegKind.Transportation | RouteLegKind.WayBack);

            Assert.Equal(2, result.Count);
            Assert.Equal(3, result.ElementAt(0).Id);
            Assert.Equal(4, result.ElementAt(1).Id);
        }

        protected override IRouteLegRepository CreateRepository(ApplicationContext context)
        {
            return new RouteLegRepository(context);
        }
    }
}