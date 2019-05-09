using Common.Models.Units;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Direction.Providers;
using TransportSystems.Backend.External.Interfaces.Direction;
using TransportSystems.Backend.External.Models.Routing;
using Xunit;

namespace TransportSystems.Backend.External.Business.Tests.Direction
{
    public class GoogleDirectionTestSuite
    {
        public GoogleDirectionTestSuite()
        {
            GoogleDirection = new GoogleDirection();
        }

        public IDirection GoogleDirection { get; }
    }

    public class GoogleDirectionTests
    {
        public GoogleDirectionTests()
        {
            Suite = new GoogleDirectionTestSuite();
        }

        public GoogleDirectionTestSuite Suite { get; }

        [Fact]
        public async Task GetRouteOfThreeLegs()
        {
            var rootAddress = new Coordinate { Latitude = 57.626569, Longitude = 39.893787 };
            var waypoints = new List<Coordinate>
            {
                new Coordinate { Latitude = 57.6525644, Longitude = 39.72409 },
                new Coordinate { Latitude = 57.8688, Longitude = 39.530759 }
            };

            var result = await Suite.GoogleDirection.GetRoute(rootAddress, rootAddress, waypoints);

            Assert.NotNull(result);
            Assert.Equal(3, result.Legs.Count);
        }

        [Fact]
        public async Task GetCuncurrentRoutes()
        {
            var waypoints = new List<Coordinate>
            {
                new Coordinate { Latitude = 57.626569, Longitude = 39.893787 },
                new Coordinate { Latitude = 57.8688, Longitude = 39.530759 }
            };

            var rootAddresses = new List<Coordinate>
            {
                new Coordinate { Latitude = 57.6525644, Longitude = 39.724092 },
                new Coordinate { Latitude = 58.0610321, Longitude = 38.7416854 },
                new Coordinate { Latitude = 59.2221979, Longitude = 39.8057537 },
                new Coordinate { Latitude = 59.1291174, Longitude = 37.7098701 }
            };

            var result = new ConcurrentBag<RouteEM>();
            var exceptions = new ConcurrentQueue<Exception>();

            await rootAddresses.ParallelForEachAsync(
                async rootAddress =>
                {
                    try
                    {
                        var route = await Suite.GoogleDirection.GetRoute(rootAddress, rootAddress, waypoints);
                        if (route.Legs.Any())
                        {
                            result.Add(route);
                        }
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                }
            );

            Assert.Equal(rootAddresses.Count, result.Count());
        }
    }
}