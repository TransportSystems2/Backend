using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Geocoder;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using TransportSystems.Backend.External.Interfaces.Services;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Geo;
using Xunit;

namespace TransportSystems.Backend.External.UnitTests.Business
{
    public class GeocoderServiceTestSuite
    {
        public GeocoderServiceTestSuite()
        {
            GeocoderAccessor = new Mock<IGeocoderAccessor>();
            GeocoderService = new GeocoderService(GeocoderAccessor.Object);
        }

        public Mock<IGeocoderAccessor> GeocoderAccessor { get; }

        public IGeocoderService GeocoderService { get; }
    }

    public class GeocoderServiceTests
    {
        [Fact]
        public async Task GetForwardAddressWhenProviderIsSpecified()
        {
            var suite = new GeocoderServiceTestSuite();

            var request = "Рыбинск";
            var maxResultCount = 5;
            var googleGeocoderResult = new List<AddressEM> { new AddressEM() };
            var googleGeocoderMock = new Mock<IGeocoder>();

            googleGeocoderMock
                .Setup(m => m.Geocode(request, maxResultCount))
                .ReturnsAsync(googleGeocoderResult);

            suite.GeocoderAccessor
                .Setup(m => m.GetProvider(ProviderKind.Google))
                .Returns(googleGeocoderMock.Object);

            var address = await suite.GeocoderService.Geocode(request, maxResultCount, ProviderKind.Google);

            suite.GeocoderAccessor
                .Verify(m => m.GetProvider(It.IsAny<ProviderKind>()));
            suite.GeocoderAccessor
                .Verify(m => m.GetProvider(ProviderKind.Google));

            googleGeocoderMock
                .Verify(m => m.Geocode(request, maxResultCount));
        }

        [Fact]
        public async Task GetForwardAddressWhenFirstDefaultProviderHasSuccessResult()
        {
            var suite = new GeocoderServiceTestSuite();

            var request = "Рыбинск";
            var maxResultCount = 7;
            var firstGeocoderResult = new List<AddressEM> { new AddressEM() };
            var firstGeocoderMock = new Mock<IGeocoder>();

            firstGeocoderMock
                .Setup(m => m.Geocode(request, maxResultCount))
                .ReturnsAsync(firstGeocoderResult);

            suite.GeocoderAccessor
                .Setup(m => m.GetProvider(suite.GeocoderService.DefaultProvidersKind[0]))
                .Returns(firstGeocoderMock.Object);

            var address = await suite.GeocoderService.Geocode(request, maxResultCount);

            suite.GeocoderAccessor
                .Verify(m => m.GetProvider(It.IsAny<ProviderKind>()), Times.Once);

            firstGeocoderMock
                .Verify(m => m.Geocode(request, maxResultCount), Times.Once);
        }

        [Fact]
        public async Task GetForwardAddressWhenSecondDefaultProviderHasSuccessResult()
        {
            var suite = new GeocoderServiceTestSuite();

            var request = "Рыбинск";
            var maxResultCount = 7;
            var firstGeocoderResult = new List<AddressEM> ();
            var firstGeocoderMock = new Mock<IGeocoder>();

            var secondGeocoderResult = new List<AddressEM> { new AddressEM() };
            var secondGeocoderMock = new Mock<IGeocoder>();

            firstGeocoderMock
                .Setup(m => m.Geocode(request, maxResultCount))
                .ReturnsAsync(firstGeocoderResult);

            secondGeocoderMock
                .Setup(m => m.Geocode(request, maxResultCount))
                .ReturnsAsync(secondGeocoderResult);

            suite.GeocoderAccessor
                .Setup(m => m.GetProvider(suite.GeocoderService.DefaultProvidersKind[0]))
                .Returns(firstGeocoderMock.Object);

            suite.GeocoderAccessor
                .Setup(m => m.GetProvider(suite.GeocoderService.DefaultProvidersKind[1]))
                .Returns(secondGeocoderMock.Object);

            var address = await suite.GeocoderService.Geocode(request, maxResultCount);

            suite.GeocoderAccessor
                .Verify(m => m.GetProvider(It.IsAny<ProviderKind>()), Times.Exactly(2));

            firstGeocoderMock
                .Verify(m => m.Geocode(request, maxResultCount), Times.Once);

            secondGeocoderMock
                .Verify(m => m.Geocode(request, maxResultCount), Times.Once);
        }

        [Fact]
        public async Task ReverseGeocodeWhenFirstDefaultProviderHasSuccessResult()
        {
            var suite = new GeocoderServiceTestSuite();

            var latitude = 52.455623;
            var longitude = 36.124467;
            var firstGeocoderResult = new List<AddressEM> { new AddressEM() };
            var firstGeocoderMock = new Mock<IGeocoder>();

            firstGeocoderMock
                .Setup(m => m.ReverseGeocode(latitude, longitude))
                .ReturnsAsync(firstGeocoderResult);

            suite.GeocoderAccessor
                .Setup(m => m.GetProvider(suite.GeocoderService.DefaultProvidersKind[0]))
                .Returns(firstGeocoderMock.Object);

            var address = await suite.GeocoderService.ReverseGeocode(latitude, longitude);

            suite.GeocoderAccessor
                .Verify(m => m.GetProvider(It.IsAny<ProviderKind>()), Times.Once);

            firstGeocoderMock
                .Verify(m => m.ReverseGeocode(latitude, longitude), Times.Once);
        }

        [Fact]
        public async Task ReverseGeocodeWhenSecondDefaultProviderHasSuccessResult()
        {
            var suite = new GeocoderServiceTestSuite();

            var latitude = 52.455623;
            var longitude = 36.124467;
            var firstGeocoderResult = new List<AddressEM>();
            var firstGeocoderMock = new Mock<IGeocoder>();

            var secondGeocoderResult = new List<AddressEM> { new AddressEM() };
            var secondGeocoderMock = new Mock<IGeocoder>();

            firstGeocoderMock
                .Setup(m => m.ReverseGeocode(latitude, longitude))
                .ReturnsAsync(firstGeocoderResult);

            secondGeocoderMock
                .Setup(m => m.ReverseGeocode(latitude, longitude))
                .ReturnsAsync(secondGeocoderResult);

            suite.GeocoderAccessor
                .Setup(m => m.GetProvider(suite.GeocoderService.DefaultProvidersKind[0]))
                .Returns(firstGeocoderMock.Object);

            suite.GeocoderAccessor
                .Setup(m => m.GetProvider(suite.GeocoderService.DefaultProvidersKind[1]))
                .Returns(secondGeocoderMock.Object);

            var address = await suite.GeocoderService.ReverseGeocode(latitude, longitude);

            suite.GeocoderAccessor
                .Verify(m => m.GetProvider(It.IsAny<ProviderKind>()), Times.Exactly(2));

            firstGeocoderMock
                .Verify(m => m.ReverseGeocode(latitude, longitude), Times.Once);

            secondGeocoderMock
                .Verify(m => m.ReverseGeocode(latitude, longitude), Times.Once);
        }
    }
}