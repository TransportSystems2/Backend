using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Routing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Data.Routing
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

        protected override IRouteLegRepository CreateRepository(ApplicationContext context)
        {
            return new RouteLegRepository(context);
        }
    }
}