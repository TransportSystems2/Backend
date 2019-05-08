using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Geocoder.Providers;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using Xunit;

namespace TransportSystems.Backend.External.Business.Tests.Geocoder
{
    public class GoogleGeocoderTests
    {
        public GoogleGeocoderTests()
        {
            Geocoder = new GoogleGeocoder();
        }

        public IGeocoder Geocoder { get; }

        [Fact]
        public async Task GeocodeCity()
        {
            var result = await Geocoder.Geocode("Ярославль Серова 5");
            var firstAddress = result.FirstOrDefault();

            Assert.Equal("Россия", firstAddress.Country);
            Assert.Equal("Ярославская область", firstAddress.Province);
            Assert.Equal("город Ярославль", firstAddress.Area);
            Assert.Equal("Ярославль", firstAddress.Locality);
            Assert.Equal("улица Серова", firstAddress.Street);
            Assert.Equal("5", firstAddress.House);
            Assert.Equal(57.609274, firstAddress.Latitude);
            Assert.Equal(39.803979, firstAddress.Longitude);
        }

        [Fact]
        public async Task GeocodeSettlement()
        {
            var result = await Geocoder.Geocode("Тутаев Привокзальная 22");
            var firstAddress = result.FirstOrDefault();

            Assert.Equal("Россия", firstAddress.Country);
            Assert.Equal("Ярославская область", firstAddress.Province);
            Assert.Equal("город Тутаев", firstAddress.Area);
            Assert.Equal("Тутаев", firstAddress.Locality);
            Assert.Equal("Привокзальная улица", firstAddress.Street);
            Assert.Equal("22", firstAddress.House);
            Assert.Equal(57.8709797, firstAddress.Latitude);
            Assert.Equal(39.5239717, firstAddress.Longitude);
        }
    }
}
