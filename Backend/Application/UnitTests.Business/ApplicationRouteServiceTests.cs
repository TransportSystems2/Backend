using Common.Models.Units;
using DotNetDistance;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using TransportSystems.Backend.External.Interfaces.Services.Direction;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Routing;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business
{
    public class RouteServiceTestsSuite : TransactionTestsSuite
    {
        public RouteServiceTestsSuite()
        {
            DirectionServiceMock = new Mock<IDirectionService>();
            DomainRouteServiceMock = new Mock<IRouteService>();
            DomainRouteLegServiceMock = new Mock<IRouteLegService>();
            AddressServiceMock = new Mock<IApplicationAddressService>();

            Service = new ApplicationRouteService(
                TransactionServiceMock.Object,
                DirectionServiceMock.Object,
                DomainRouteServiceMock.Object,
                DomainRouteLegServiceMock.Object,
                AddressServiceMock.Object);
        }

        public IApplicationRouteService Service { get; }

        public Mock<IDirectionService> DirectionServiceMock { get; } 

        public Mock<IRouteService> DomainRouteServiceMock { get; }

        public Mock<IRouteLegService> DomainRouteLegServiceMock { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }
    }

    public class ApplicationRouteServiceTests : BaseServiceTests<RouteServiceTestsSuite>
    {
        [Fact]
        public async Task CreateDomainRoute()
        {
            var commonId = 1;

            var rootAddress = new AddressAM
            {
                Latitude = 55.55555,
                Longitude = 66.66666
            };

            var firstWaypointAddress = new AddressAM
            {
                Latitude = 11.11111,
                Longitude = 22.22222,
                AdjustedLatitude = 11.11000,
                AdjustedLongitude = 22.22000
            };

            var secondWaypointAddress = new AddressAM
            {
                Latitude = 33.33333,
                Longitude = 44.44444,
                AdjustedLatitude = 33.33000,
                AdjustedLongitude = 44.44000
            };

            var route = new RouteAM
            {
                Comment = "It is customer comment",
                Legs = {
                    new RouteLegAM
                    {
                        StartAddress = rootAddress,
                        EndAddress = firstWaypointAddress,
                        Distance = Distance.FromKilometers(30),
                        Duration = TimeSpan.FromMinutes(30),
                        Kind = RouteLegKind.Feed
                    },
                    new RouteLegAM
                    {
                        StartAddress = firstWaypointAddress,
                        EndAddress = secondWaypointAddress,
                        Distance = Distance.FromKilometers(35),
                        Duration = TimeSpan.FromMinutes(35),
                        Kind = RouteLegKind.Transportation
                    },
                    new RouteLegAM
                    {
                        StartAddress = secondWaypointAddress,
                        EndAddress = rootAddress,
                        Distance = Distance.FromKilometers(38),
                        Duration = TimeSpan.FromMinutes(38),
                        Kind = RouteLegKind.WayBack
                    }
                }
            };

            var domainRoute = new Route { Id = commonId++ };
            var domainRouteAddress = new Address { Id = commonId++ };
            var firstDomainWaypointAddress = new Address { Id = commonId++ };
            var secondDomainWaypoinAddress = new Address { Id = commonId++ };

            Suite.AddressServiceMock
                .Setup(m => m.GetOrCreateDomainAddress(rootAddress))
                .ReturnsAsync(domainRouteAddress);
            Suite.AddressServiceMock
                .Setup(m => m.GetOrCreateDomainAddress(firstWaypointAddress))
                .ReturnsAsync(firstDomainWaypointAddress);
            Suite.AddressServiceMock
                .Setup(m => m.GetOrCreateDomainAddress(secondWaypointAddress))
                .ReturnsAsync(secondDomainWaypoinAddress);

            Suite.DomainRouteServiceMock
                .Setup(m => m.Create(route.Comment))
                .ReturnsAsync(domainRoute);

            var result = await Suite.Service.CreateDomainRoute(route);
            
            Suite.DomainRouteServiceMock
                .Verify(m => m.Create(route.Comment));

            var firstLeg = route.Legs[0];
            Suite.DomainRouteLegServiceMock
                .Verify(m => m.Create(
                    domainRoute.Id,
                    firstLeg.Kind,
                    domainRouteAddress.Id,
                    firstDomainWaypointAddress.Id,
                    firstLeg.Duration,
                    firstLeg.Distance));

            var secondLeg = route.Legs[1];
            Suite.DomainRouteLegServiceMock
                .Verify(m => m.Create(
                    domainRoute.Id,
                    secondLeg.Kind,
                    firstDomainWaypointAddress.Id,
                    secondDomainWaypoinAddress.Id,
                    secondLeg.Duration,
                    secondLeg.Distance));

            var thirdLeg = route.Legs[2];
            Suite.DomainRouteLegServiceMock
                .Verify(m => m.Create(
                    domainRoute.Id,
                    thirdLeg.Kind,
                    secondDomainWaypoinAddress.Id,
                    domainRouteAddress.Id,
                    thirdLeg.Duration,
                    thirdLeg.Distance));
        }

        [Fact]
        public async Task GetRoute()
        {
            var rootAddress = new AddressAM
            {
                Latitude = 55.55555,
                Longitude = 66.66666
            };

            var firstWaypointAddress = new AddressAM
            {
                Latitude = 11.11111,
                Longitude = 22.22222
            };

            var secondWaypointAddress = new AddressAM
            {
                Latitude = 33.33333,
                Longitude = 44.44444
            };

            var waypoints = new WaypointsAM
            {
                Points = new List<AddressAM>
                {
                    firstWaypointAddress,
                    secondWaypointAddress
                },
                Comment = "Машина на охраняемой парковке. Пароль:\"Нраииттьься\""
            };

            var waypointsCoordinate = waypoints.Points.Select(p => p.ToCoordinate());

            var rootCoordinate = new Coordinate { Latitude = 55.55000, Longitude = 66.66000 };
            var firstWaypointCoordinate = new Coordinate { Latitude = 11.11000, Longitude = 22.22000 };
            var secondWaypointCoordinate = new Coordinate { Latitude = 33.33000, Longitude = 44.44000 };

            var externalRoute = new RouteEM
            {
                Status = External.Models.Enums.Status.Ok,
                Legs = new List<LegEM>
                {
                    new LegEM
                    {
                        StartCoordinate = rootCoordinate,
                        EndCoordinate = firstWaypointCoordinate,
                        Distance = Distance.FromKilometers(3),
                        Duration = TimeSpan.FromMinutes(10)
                    },
                    new LegEM
                    {
                        StartCoordinate = firstWaypointCoordinate,
                        EndCoordinate = secondWaypointCoordinate,
                        Distance = Distance.FromKilometers(35),
                        Duration = TimeSpan.FromMinutes(45)
                    },
                    new LegEM
                    {
                        StartCoordinate = secondWaypointCoordinate,
                        EndCoordinate = rootCoordinate,
                        Distance = Distance.FromKilometers(38),
                        Duration = TimeSpan.FromMinutes(48)
                    }
                }
            };

            Suite.DirectionServiceMock
                .Setup(m => m.GetRoute(
                    It.IsAny<Coordinate>(),
                    It.IsAny<Coordinate>(),
                    It.IsAny<IEnumerable<Coordinate>>()))
                .ReturnsAsync(externalRoute);

            Suite.AddressServiceMock
                .Setup(m => m.GetNearestAddress(rootCoordinate, It.IsAny<IEnumerable<AddressAM>>()))
                .ReturnsAsync(rootAddress);
            Suite.AddressServiceMock
                .Setup(m => m.GetNearestAddress(firstWaypointCoordinate, It.IsAny<IEnumerable<AddressAM>>()))
                .ReturnsAsync(firstWaypointAddress);
            Suite.AddressServiceMock
                .Setup(m => m.GetNearestAddress(secondWaypointCoordinate, It.IsAny<IEnumerable<AddressAM>>()))
                .ReturnsAsync(secondWaypointAddress);

            var result = await Suite.Service.GetRoute(rootAddress, waypoints);

            Assert.Equal(3, result.Legs.Count);

            // Check feeding leg
            Assert.Equal(rootAddress, result.Legs[0].StartAddress);
            Assert.Equal(firstWaypointAddress, result.Legs[0].EndAddress);
            Assert.Equal(externalRoute.Legs[0].Distance, result.Legs[0].Distance);
            Assert.Equal(externalRoute.Legs[0].Duration, result.Legs[0].Duration);
            Assert.Equal(RouteLegKind.Feed, result.Legs[0].Kind);

            // Check transportation leg
            Assert.Equal(firstWaypointAddress, result.Legs[1].StartAddress);
            Assert.Equal(secondWaypointAddress, result.Legs[1].EndAddress);
            Assert.Equal(externalRoute.Legs[1].Distance, result.Legs[1].Distance);
            Assert.Equal(externalRoute.Legs[1].Duration, result.Legs[1].Duration);
            Assert.Equal(RouteLegKind.Transportation, result.Legs[1].Kind);

            // Check wayBacking leg
            Assert.Equal(secondWaypointAddress, result.Legs[2].StartAddress);
            Assert.Equal(rootAddress, result.Legs[2].EndAddress);
            Assert.Equal(externalRoute.Legs[2].Distance, result.Legs[2].Distance);
            Assert.Equal(externalRoute.Legs[2].Duration, result.Legs[2].Duration);
            Assert.Equal(RouteLegKind.WayBack, result.Legs[2].Kind);

            Assert.Equal(rootCoordinate.Latitude, result.Legs[0].StartAddress.AdjustedLatitude);
            Assert.Equal(rootCoordinate.Longitude, result.Legs[0].StartAddress.AdjustedLongitude);

            Assert.Equal(firstWaypointCoordinate.Latitude, result.Legs[1].StartAddress.AdjustedLatitude);
            Assert.Equal(firstWaypointCoordinate.Longitude, result.Legs[1].StartAddress.AdjustedLongitude);

            Assert.Equal(secondWaypointCoordinate.Latitude, result.Legs[2].StartAddress.AdjustedLatitude);
            Assert.Equal(secondWaypointCoordinate.Longitude, result.Legs[2].StartAddress.AdjustedLongitude);
        }

        [Fact]
        public async Task GetPossibleRoutes()
        {
            var coordinate = new Coordinate { Latitude = 1, Longitude = 1 };
            var waypoints = new WaypointsAM
            {
                Points = new List<AddressAM>
                {
                    new AddressAM { Latitude = 1, Longitude = 1 },
                    new AddressAM { Latitude = 1, Longitude = 1 }
                }
            };

            var rootAddresses = new List<AddressAM>
            {
                new AddressAM { Latitude = 1, Longitude = 1 },
                new AddressAM { Latitude = 1, Longitude = 1 },
                new AddressAM { Latitude = 1, Longitude = 1 },
                new AddressAM { Latitude = 1, Longitude = 1 }
            };

            Suite.DirectionServiceMock
                .Setup(m => m.GetRoute(
                    It.IsAny<Coordinate>(),
                    It.IsAny<Coordinate>(),
                    It.IsAny<IEnumerable<Coordinate>>(),
                    It.IsAny<ProviderKind[]>()))
                .ReturnsAsync(() =>
                {
                    return new RouteEM
                    {
                        Status = Status.Ok,
                        Legs = new List<LegEM>
                            {
                                new LegEM
                                {
                                    StartCoordinate = coordinate,
                                    EndCoordinate = coordinate
                                },
                                new LegEM
                                {
                                    StartCoordinate = coordinate,
                                    EndCoordinate = coordinate
                                },
                                new LegEM
                                {
                                    StartCoordinate = coordinate,
                                    EndCoordinate = coordinate
                                }
                            }
                    };
                });
            Suite.AddressServiceMock
                .Setup(m => m.GetNearestAddress(coordinate, It.IsAny<IEnumerable<AddressAM>>()))
                .ReturnsAsync(new AddressAM { Latitude = 1, Longitude = 1 });

            var result = await Suite.Service.GetPossibleRoutes(rootAddresses, waypoints);

            Assert.Equal(rootAddresses.Count, result.Count);
        }

        [Fact]
        public async Task FromExternalRoute()
        {
            var rootAddress = new AddressAM
            {
                Latitude = 55.55555,
                Longitude = 66.66666
            };

            var firstWaypointAddress = new AddressAM
            {
                Latitude = 11.11111,
                Longitude = 22.22222
            };

            var secondWaypointAddress = new AddressAM
            {
                Latitude = 33.33333,
                Longitude = 44.44444
            };

            var waypoints = new WaypointsAM
            {
                Points = new List<AddressAM>
                {
                    firstWaypointAddress,
                    secondWaypointAddress    
                },
                Comment = "Машина на охраняемой парковке. Пароль:\"Нраииттьься\""
            };

            var rootCoordinate = new Coordinate { Latitude = 55.55000, Longitude = 66.66000 };
            var firstWaypointCoordinate = new Coordinate { Latitude = 11.11000, Longitude = 22.22000 };
            var secondWaypointCoordinate = new Coordinate { Latitude = 33.33000, Longitude = 44.44000 };

            var externalRoute = new RouteEM
            {
                Status = External.Models.Enums.Status.Ok,
                Legs = new List<LegEM>
                {
                    new LegEM
                    {
                        StartCoordinate = rootCoordinate,
                        EndCoordinate = firstWaypointCoordinate,
                        Distance = Distance.FromKilometers(3),
                        Duration = TimeSpan.FromMinutes(10)
                    },
                    new LegEM
                    {
                        StartCoordinate = firstWaypointCoordinate,
                        EndCoordinate = secondWaypointCoordinate,
                        Distance = Distance.FromKilometers(35),
                        Duration = TimeSpan.FromMinutes(46)
                    },
                    new LegEM
                    {
                        StartCoordinate = secondWaypointCoordinate,
                        EndCoordinate = rootCoordinate,
                        Distance = Distance.FromKilometers(38),
                        Duration = TimeSpan.FromMinutes(48)
                    }
                }
            };

            Suite.AddressServiceMock
                .Setup(m => m.GetNearestAddress(rootCoordinate, It.IsAny<IEnumerable<AddressAM>>()))
                .ReturnsAsync(rootAddress);
            Suite.AddressServiceMock
                .Setup(m => m.GetNearestAddress(firstWaypointCoordinate, It.IsAny<IEnumerable<AddressAM>>()))
                .ReturnsAsync(firstWaypointAddress);
            Suite.AddressServiceMock
                .Setup(m => m.GetNearestAddress(secondWaypointCoordinate, It.IsAny<IEnumerable<AddressAM>>()))
                .ReturnsAsync(secondWaypointAddress);

            var result = await Suite.Service.FromExternalRoute(rootAddress, rootAddress, waypoints, externalRoute);

            Assert.Equal(3, result.Legs.Count);

            // Check feeding leg
            Assert.Equal(rootAddress, result.Legs[0].StartAddress);
            Assert.Equal(firstWaypointAddress, result.Legs[0].EndAddress);
            Assert.Equal(externalRoute.Legs[0].Distance, result.Legs[0].Distance);
            Assert.Equal(externalRoute.Legs[0].Duration, result.Legs[0].Duration);
            Assert.Equal(RouteLegKind.Feed, result.Legs[0].Kind);

            // Check transportation leg
            Assert.Equal(firstWaypointAddress, result.Legs[1].StartAddress);
            Assert.Equal(secondWaypointAddress, result.Legs[1].EndAddress);
            Assert.Equal(externalRoute.Legs[1].Distance, result.Legs[1].Distance);
            Assert.Equal(externalRoute.Legs[1].Duration, result.Legs[1].Duration);
            Assert.Equal(RouteLegKind.Transportation, result.Legs[1].Kind);

            // Check wayBacking leg
            Assert.Equal(secondWaypointAddress, result.Legs[2].StartAddress);
            Assert.Equal(rootAddress, result.Legs[2].EndAddress);
            Assert.Equal(externalRoute.Legs[2].Distance, result.Legs[2].Distance);
            Assert.Equal(externalRoute.Legs[2].Duration, result.Legs[2].Duration);
            Assert.Equal(RouteLegKind.WayBack, result.Legs[2].Kind);

            Assert.Equal(rootCoordinate.Latitude, result.Legs[0].StartAddress.AdjustedLatitude);
            Assert.Equal(rootCoordinate.Longitude, result.Legs[0].StartAddress.AdjustedLongitude);

            Assert.Equal(firstWaypointCoordinate.Latitude, result.Legs[1].StartAddress.AdjustedLatitude);
            Assert.Equal(firstWaypointCoordinate.Longitude, result.Legs[1].StartAddress.AdjustedLongitude);

            Assert.Equal(secondWaypointCoordinate.Latitude, result.Legs[2].StartAddress.AdjustedLatitude);
            Assert.Equal(secondWaypointCoordinate.Longitude, result.Legs[2].StartAddress.AdjustedLongitude);
        }

        [Fact]
        public async Task GetShortTitleWithTheSameAddressShortNameResultNameConsistOnePart()
        {
            var routeLegKind = RouteLegKind.Transportation;
            var routeLegs = new List<RouteLeg>
            {
                new RouteLeg
                {
                    Id = 5,
                    RouteId = 1,
                    StartAddressId = 2,
                    EndAddressId = 3,
                    Kind = routeLegKind
                }
            };

            var route = new Route { Id = 1 };

            var startAddressShortTitle = "Москва";
            var endAddressShortTitle = "Москва";

            Suite.DomainRouteServiceMock
                .Setup(m => m.Get(route.Id))
                .ReturnsAsync(route);

            Suite.DomainRouteLegServiceMock
                 .Setup(m => m.GetByRoute(route.Id, routeLegKind))
                 .ReturnsAsync(routeLegs);

            Suite.AddressServiceMock
                .Setup(m => m.GetShortTitle(routeLegs.First().StartAddressId))
                .ReturnsAsync(startAddressShortTitle);

            Suite.AddressServiceMock
                .Setup(m => m.GetShortTitle(routeLegs.First().EndAddressId))
                .ReturnsAsync(endAddressShortTitle);

            var shortName = await Suite.Service.GetShortTitle(route.Id);

            Assert.Equal("Москва", shortName);
        }

        [Fact]
        public async Task GetShortTitleWithDifferentAddressShortNameResultNameConsistsTwoParts()
        {
            var routeLegKind = RouteLegKind.Transportation;
            var routeLegs = new List<RouteLeg>
            {
                new RouteLeg
                {
                    Id = 5,
                    RouteId = 1,
                    StartAddressId = 2,
                    EndAddressId = 3,
                    Kind = routeLegKind
                }
            };

            var route = new Route { Id = 1 };

            var startAddressShortTitle = "Москва";
            var endAddressShortTitle = "Рыбинск";

            Suite.DomainRouteServiceMock
                .Setup(m => m.Get(route.Id))
                .ReturnsAsync(route);

            Suite.DomainRouteLegServiceMock
                 .Setup(m => m.GetByRoute(route.Id, routeLegKind))
                 .ReturnsAsync(routeLegs);

            Suite.AddressServiceMock
                .Setup(m => m.GetShortTitle(routeLegs.First().StartAddressId))
                .ReturnsAsync(startAddressShortTitle);

            Suite.AddressServiceMock
                .Setup(m => m.GetShortTitle(routeLegs.First().EndAddressId))
                .ReturnsAsync(endAddressShortTitle);

            var shortTitle = await Suite.Service.GetShortTitle(route.Id);

            var expectedShortTitle = string.Join(" - ", startAddressShortTitle, endAddressShortTitle);
            Assert.Equal(expectedShortTitle, shortTitle);
        }

        [Fact]
        public async Task GetShortTitleWithDifferentAddressShortNameResultNameConsistsThreeParts()
        {
            var routeLegKind = RouteLegKind.Transportation;

            var routeLegs = new List<RouteLeg>
            {
                new RouteLeg { Id = 5, RouteId = 1, StartAddressId = 2, EndAddressId = 3, Kind = RouteLegKind.Transportation},
                new RouteLeg { Id = 6, RouteId = 1, StartAddressId = 3, EndAddressId = 4, Kind = RouteLegKind.Transportation}
            };

            var route = new Route { Id = 1 };

            var startAddressShortTitle = "Москва";
            var middleAddressShortTitle = "Рыбинск";
            var endAddressShortTitle = "Ярославль";

            Suite.DomainRouteServiceMock
                .Setup(m => m.Get(route.Id))
                .ReturnsAsync(route);

            Suite.DomainRouteLegServiceMock
                 .Setup(m => m.GetByRoute(route.Id, routeLegKind))
                 .ReturnsAsync(routeLegs);

            Suite.AddressServiceMock
                .Setup(m => m.GetShortTitle(2))
                .ReturnsAsync(startAddressShortTitle);

            Suite.AddressServiceMock
                .Setup(m => m.GetShortTitle(3))
                .ReturnsAsync(middleAddressShortTitle);

            Suite.AddressServiceMock
                .Setup(m => m.GetShortTitle(4))
                .ReturnsAsync(endAddressShortTitle);

            var shortTitle = await Suite.Service.GetShortTitle(route.Id);

            var expectedShortTitle = string.Join(" - ", startAddressShortTitle, middleAddressShortTitle, endAddressShortTitle);
            Assert.Equal(expectedShortTitle, shortTitle);
        }

        [Fact]
        public void GetRootAddress()
        {
            var rootAddress = new AddressAM();
            var route = new RouteAM
            {
                Comment = "It is customer comment",
                Legs = {
                    new RouteLegAM
                    {
                        StartAddress = rootAddress,
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(30),
                        Duration = TimeSpan.FromMinutes(10),
                        Kind = RouteLegKind.Feed
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(35),
                        Duration = TimeSpan.FromMinutes(35),
                        Kind = RouteLegKind.Transportation
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = rootAddress,
                        Distance = Distance.FromKilometers(38),
                        Duration = TimeSpan.FromMinutes(48),
                        Kind = RouteLegKind.WayBack
                    }
                }
            };

            var result = Suite.Service.GetRootAddress(route);

            Assert.Equal(rootAddress, result);
        }

        [Fact]
        public void GetTotalDistance()
        {
            var route = new RouteAM
            {
                Comment = "It is customer comment",
                Legs = {
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(10),
                        Duration = TimeSpan.FromMinutes(9),
                        Kind = RouteLegKind.Feed
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(35),
                        Duration = TimeSpan.FromMinutes(35),
                        Kind = RouteLegKind.Transportation
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(38),
                        Duration = TimeSpan.FromMinutes(38),
                        Kind = RouteLegKind.WayBack
                    }
                }
            };

            var result = Suite.Service.GetTotalDistance(route);

            Assert.Equal(83, result.ToKilometers());
        }

        [Fact]
        public void GetFeedDistance()
        {
            var route = new RouteAM
            {
                Comment = "It is customer comment",
                Legs = {
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(30),
                        Duration = TimeSpan.FromMinutes(30),
                        Kind = RouteLegKind.Feed
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(35),
                        Duration = TimeSpan.FromMinutes(34),
                        Kind = RouteLegKind.Transportation
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(38),
                        Duration = TimeSpan.FromMinutes(48),
                        Kind = RouteLegKind.WayBack
                    }
                }
            };

            var result = Suite.Service.GetFeedDistance(route);

            Assert.Equal(Distance.FromKilometers(30), result);
        }

        [Fact]
        public async Task GetFeedDuration()
        {
            var route = new RouteAM
            {
                Comment = "It is customer comment",
                Legs = {
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(30),
                        Duration = TimeSpan.FromMinutes(12),
                        Kind = RouteLegKind.Feed
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(35),
                        Duration = TimeSpan.FromMinutes(38),
                        Kind = RouteLegKind.Transportation
                    },
                    new RouteLegAM
                    {
                        StartAddress = new AddressAM(),
                        EndAddress = new AddressAM(),
                        Distance = Distance.FromKilometers(38),
                        Duration = TimeSpan.FromMinutes(48),
                        Kind = RouteLegKind.WayBack
                    }
                }
            };

            var result = await Suite.Service.GetFeedDuration(route);

            Assert.Equal(route.Legs[0].Duration, result);
        }

        [Fact]
        public async Task GetDistance()
        {
            var routeId = 1;
            var totalDistance = Distance.FromKilometers(100);

            Suite.DomainRouteLegServiceMock
                .Setup(m => m.GetDistance(routeId, RouteLegKind.All))
                .ReturnsAsync(totalDistance);

            var result = await Suite.Service.GetTotalDistance(routeId);

            Assert.Equal(totalDistance, result);
        }
    }
}