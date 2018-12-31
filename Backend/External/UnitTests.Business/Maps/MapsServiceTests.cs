using Common.Models;
using Common.Models.Geolocation;
using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Maps;
using TransportSystems.Backend.External.Interfaces.Exceptions;
using TransportSystems.Backend.External.Interfaces.Maps;
using TransportSystems.Backend.External.Interfaces.Services.Maps;
using TransportSystems.Backend.External.Models.Enums;
using Xunit;

namespace TransportSystems.Backend.External.UnitTests.Business.Maps
{
    public class MapsServiceTestSuite
    {
        public MapsServiceTestSuite()
        {
            MapsAccessorMock = new Mock<IMapsAccessor>();
            MapsService = new MapsService(MapsAccessorMock.Object);
        }

        public Mock<IMapsAccessor> MapsAccessorMock { get; }

        public IMapsService MapsService { get; }
    }

    public class MapsServiceTests
    {
        public MapsServiceTests()
        {
            Suite = new MapsServiceTestSuite();
        }

        protected MapsServiceTestSuite Suite { get; }

        [Fact]
        public async Task GetTimeBeltWhenProviderIsSpecified()
        {
            var googleMapsMock = new Mock<IMaps>();
            var providerKind = ProviderKind.Google;
            var coordinate = new Coordinate { Latitude = 1, Longitude = 2 };
            var timeBelt = new TimeBelt
            {
                Id = "Sample",
                Name = "Sample Name",
                OffSet = TimeSpan.FromHours(1),
                RawOffset = TimeSpan.FromHours(-3)
            };

            googleMapsMock
                .Setup(m => m.GetTimeBelt(coordinate))
                .ReturnsAsync(timeBelt);

            Suite.MapsAccessorMock
                .Setup(m => m.GetProvider(providerKind))
                .Returns(googleMapsMock.Object);

            var result = await Suite.MapsService.GetTimeBelt(coordinate, providerKind);

            Suite.MapsAccessorMock
                .Verify(m => m.GetProvider(providerKind));

            googleMapsMock
                .Verify(m => m.GetTimeBelt(coordinate));

            Assert.Equal(timeBelt, result);
        }

        [Fact]
        public async Task GetTimeBeltWhenProvidersDoesNotExist()
        {
            var coordinate = new Coordinate { Latitude = 1, Longitude = 2 };
            var providerKind = ProviderKind.Yandex;

            Suite.MapsAccessorMock
                .Setup(m => m.GetProvider(providerKind))
                .Returns((IMaps)null);

            await Assert.ThrowsAsync<ArgumentException>(
                "ProvidersKind",
                () => Suite.MapsService.GetTimeBelt(coordinate, providerKind));

            Suite.MapsAccessorMock
                .Verify(m => m.GetProvider(providerKind));
        }

        [Fact]
        public async Task GetTimeBeltWhenResultDoesNotExist()
        {
            var googleMapsMock = new Mock<IMaps>();
            var providerKind = ProviderKind.Google;
            var coordinate = new Coordinate { Latitude = 1, Longitude = 2 };

            googleMapsMock
                .Setup(m => m.GetTimeBelt(coordinate))
                .ThrowsAsync(new ResponseStatusException(Status.RequestDenied));

            Suite.MapsAccessorMock
                .Setup(m => m.GetProvider(providerKind))
                .Returns(googleMapsMock.Object);

            await Assert.ThrowsAsync<AggregateException>(() => Suite.MapsService.GetTimeBelt(coordinate, providerKind));

            Suite.MapsAccessorMock
                .Verify(m => m.GetProvider(providerKind));

            googleMapsMock
                .Verify(m => m.GetTimeBelt(coordinate));
        }
    }
}