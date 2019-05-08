using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Geocoder.Providers;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using Xunit;

namespace TransportSystems.Backend.External.Business.Tests.Geocoder
{
    public class YandexGeocoderTests
    {
        public YandexGeocoderTests()
        {
            Geocoder = new YandexGeocoder();
        }

        public IGeocoder Geocoder { get; }

        [Fact]
        public async Task GeocodeCity()
        {
            var result = await Geocoder.Geocode("Ярославль Серова 5");
            var firstAddress = result.FirstOrDefault();

            Assert.Equal("Россия", firstAddress.Country);
            Assert.Equal("Ярославская область", firstAddress.Province);
            Assert.Equal("городской округ Ярославль", firstAddress.Area);
            Assert.Equal("Ярославль", firstAddress.Locality);
            Assert.Equal("улица Серова", firstAddress.Street);
            Assert.Equal("5", firstAddress.House);
            Assert.Equal(57.609206, firstAddress.Latitude);
            Assert.Equal(39.804018, firstAddress.Longitude);
        }

        [Fact]
        public async Task GeocodeSettlement()
        {
            var result = await Geocoder.Geocode("Ярославская область Октябрьский 12");
            var firstAddress = result.FirstOrDefault();

            Assert.Equal("Россия", firstAddress.Country);
            Assert.Equal("Ярославская область", firstAddress.Province);
            Assert.Equal("Рыбинский район", firstAddress.Area);
            Assert.Equal("поселок Октябрьский", firstAddress.Locality);
            Assert.Null(firstAddress.Street);
            Assert.Equal("12", firstAddress.House);
            Assert.Equal(57.984794, firstAddress.Latitude);
            Assert.Equal(39.110177, firstAddress.Longitude);
        }
    }
}