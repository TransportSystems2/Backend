using Moq;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;
using TransportSystems.Backend.Core.Infrastructure.Business.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Routing
{
    public class RouteServiceTestSuite
    {
        public RouteServiceTestSuite()
        {
            RouteRepositoryMock = new Mock<IRouteRepository>();

            RouteService = new RouteService(RouteRepositoryMock.Object);
        }

        public Mock<IRouteRepository> RouteRepositoryMock { get; }

        public IRouteService RouteService { get; }
    }

    public class RouteServiceTests
    {
    }
}