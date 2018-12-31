using Common.Models.Geolocation;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Direction;
using TransportSystems.Backend.External.Interfaces.Direction;
using TransportSystems.Backend.External.Interfaces.Services.Direction;
using Xunit;

namespace TransportSystems.Backend.External.UnitTests.Business.Direction
{
    public class DirectionServiceTestSuite
    {
        public DirectionServiceTestSuite()
        {
            DirectionAccessor = new Mock<IDirectionAccessor>();
            DirectionService = new DirectionService(DirectionAccessor.Object);
        }

        public Mock<IDirectionAccessor> DirectionAccessor { get; }

        public IDirectionService DirectionService { get; }
    }

    public class DirectionServiceTests
    {
        public DirectionServiceTests()
        {
            Suite = new DirectionServiceTestSuite();
        }

        protected DirectionServiceTestSuite Suite { get; }

        [Fact]
        public async Task GetNearestCoordinateWhereFirstIsNearest()
        {
            var originCoordinate = new Coordinate { Latitude = 58.057676, Longitude = 38.804993 };
            var coordinates = new List<Coordinate>
            {
                new Coordinate { Latitude = 58.891167, Longitude = 38.540039 },
                new Coordinate { Latitude = 58.295779, Longitude = 47.856445 },
                new Coordinate { Latitude = 55.959195, Longitude = 39.506836 },
                new Coordinate { Latitude = 57.760601, Longitude = 32.299805 }
            };

            var result = await Suite.DirectionService.GetNearestCoordinate(originCoordinate, coordinates);

            Assert.Equal(coordinates[0], result);
        }

        [Fact]
        public async Task GetNearestCoordinateWhereSecondIsNearest()
        {
            var originCoordinate = new Coordinate { Latitude = 58.057676, Longitude = 38.804993 };
            var coordinates = new List<Coordinate>
            {
                new Coordinate { Latitude = 58.891167, Longitude = 38.540039 },
                new Coordinate { Latitude = 58.047092, Longitude = 39.221191 },
                new Coordinate { Latitude = 55.959195, Longitude = 39.506836 },
                new Coordinate { Latitude = 57.760601, Longitude = 32.299805 }
            };

            var result = await Suite.DirectionService.GetNearestCoordinate(originCoordinate, coordinates);

            Assert.Equal(coordinates[1], result);
        }

        [Fact]
        public async Task GetNearestCoordinateWhereThirdIsNearest()
        {
            var targetCoordinate = new Coordinate { Latitude = 58.057676, Longitude = 38.804993 };
            var coordinates = new List<Coordinate>
            {
                new Coordinate { Latitude = 58.891167, Longitude = 38.540039 },
                new Coordinate { Latitude = 58.047092, Longitude = 39.221191 },
                new Coordinate { Latitude = 57.951029, Longitude = 38.869629 },
                new Coordinate { Latitude = 57.760601, Longitude = 32.299805 }
            };

            var result = await Suite.DirectionService.GetNearestCoordinate(targetCoordinate, coordinates);

            Assert.Equal(coordinates[2], result);
        }

        [Fact]
        public async Task GetNearestCoordinateWhereFourthIsNearest()
        {
            var originCoordinates = new Coordinate { Latitude = 58.057676, Longitude = 38.804993 };
            var coordinates = new List<Coordinate>
            {
                new Coordinate { Latitude = 58.891167, Longitude = 38.540039 },
                new Coordinate { Latitude = 58.047092, Longitude = 39.221191 },
                new Coordinate { Latitude = 55.959195, Longitude = 39.506836 },
                new Coordinate { Latitude = 58.042405, Longitude = 38.777618 }
            };

            var result = await Suite.DirectionService.GetNearestCoordinate(originCoordinates, coordinates);

            Assert.Equal(coordinates[3], result);
        }

        [Fact]
        public async Task GetCoordinateBounds()
        {
            var coordinate = new Coordinate
            {
                Latitude = 12.34567,
                Longitude = 76.54321
            };

            var distance = 500d;

            var result = await Suite.DirectionService.GetCoordinateBounds(coordinate, distance);

            Assert.Equal(coordinate.Latitude, result.Latitude);
            Assert.Equal(coordinate.Longitude, result.Longitude);
            Assert.Equal(5.099293188405797, result.MinLatitude);
            Assert.Equal(69.125297365626025, result.MinLongitude);
            Assert.Equal(19.5920468115942, result.MaxLatitude);
            Assert.Equal(83.961122634373979, result.MaxLongitude);
        }
    }
}