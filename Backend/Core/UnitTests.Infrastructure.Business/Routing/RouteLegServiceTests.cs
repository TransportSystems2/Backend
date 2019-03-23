using DotNetDistance;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;
using TransportSystems.Backend.Core.Infrastructure.Business.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Routing
{
    public class RouteLegServiceTestSuite
    {
        public RouteLegServiceTestSuite()
        {
            RouteLegRepositoryMock = new Mock<IRouteLegRepository>();
            RouteServiceMock = new Mock<IRouteService>();
            AddressServiceMock = new Mock<IAddressService>();

            RouteLegService = new RouteLegService(
                RouteLegRepositoryMock.Object,
                RouteServiceMock.Object,
                AddressServiceMock.Object);
        }
        public IRouteLegService RouteLegService { get; }

        public Mock<IRouteLegRepository> RouteLegRepositoryMock { get; }

        public Mock<IRouteService> RouteServiceMock { get; }

        public Mock<IAddressService> AddressServiceMock { get; }
    }

    public class RouteLegServiceTests
    {
        public RouteLegServiceTests()
        {
            Suite = new RouteLegServiceTestSuite();
        }

        protected RouteLegServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreateLeg()
        {
            var commonId = 1;
            var routeId = commonId++;
            var kind = RouteLegKind.Feed;
            var startAddressId = commonId++;
            var endAddressId = commonId++;
            var duration = TimeSpan.FromHours(1);
            var distance = Distance.FromKilometers(80);

            Suite.RouteServiceMock
                .Setup(m => m.IsExist(routeId))
                .ReturnsAsync(true);
            Suite.AddressServiceMock
                .Setup(m => m.IsExist(It.IsIn(startAddressId, endAddressId)))
                .ReturnsAsync(true);

            var result = await Suite.RouteLegService.Create(routeId, kind, startAddressId, endAddressId, duration, distance);

            Suite.RouteLegRepositoryMock
                .Verify(m => m.Add(result));
            Suite.RouteLegRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(routeId, result.RouteId);
            Assert.Equal(kind, result.Kind);
            Assert.Equal(startAddressId, result.StartAddressId);
            Assert.Equal(endAddressId, result.EndAddressId);
            Assert.Equal(duration, result.Duration);
            Assert.Equal(distance, result.Distance);
        }

        [Fact]
        public async Task GetLegsByRoute()
        {
            var routeId = 1;
            var routeLegKind = RouteLegKind.Transportation;

            Suite.RouteServiceMock
                .Setup(m => m.IsExist(routeId))
                .ReturnsAsync(true);

            await Suite.RouteLegService.GetByRoute(routeId, routeLegKind);

            Suite.RouteLegRepositoryMock
                .Verify(m => m.GetByRoute(routeId, routeLegKind));
        }

        [Fact]
        public async Task GetDistanceByRoute()
        {
            var commonId = 1;
            var routeId = commonId++;
            var routeLegKind = RouteLegKind.Feed;
            var legs = new List<RouteLeg>
            {
                new RouteLeg {Id = commonId++, RouteId = routeId, Distance = Distance.FromKilometers(30), Kind = RouteLegKind.Feed },
                new RouteLeg {Id = commonId++, RouteId = routeId, Distance = Distance.FromKilometers(20), Kind = RouteLegKind.Feed }
            };

            Suite.RouteServiceMock
                .Setup(m => m.IsExist(routeId))
                .ReturnsAsync(true);
            Suite.RouteLegRepositoryMock
                .Setup(m => m.GetByRoute(routeId, routeLegKind))
                .ReturnsAsync(legs);

            var result = await Suite.RouteLegService.GetDistance(routeId, routeLegKind);

            Assert.Equal(legs.Sum(l => l.Distance), result);
        }
    }
}