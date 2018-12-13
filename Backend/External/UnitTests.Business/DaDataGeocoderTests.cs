using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Geocoder.Providers;
using TransportSystems.Backend.External.Interfaces.Geocoder;
using Xunit;

namespace TransportSystems.Backend.External.UnitTests.Business
{
    public class DaDataGeocoderTests
    {
        public DaDataGeocoderTests()
        {
            Geocoder = new DaDataGeocoder();
        }

        public IGeocoder Geocoder { get; }

        [Fact]
        public async Task GeocodeCity()
        {
            var result = await Geocoder.Geocode("Ярославль Серова 5");
            var firstAddress = result.FirstOrDefault();

            Assert.Equal("Россия", firstAddress.Country);
            Assert.Equal("Ярославская обл", firstAddress.Province);
            Assert.Null(firstAddress.Area);
            Assert.Equal("г Ярославль", firstAddress.Locality);
            Assert.Equal("ул Серова", firstAddress.Street);
            Assert.Equal("5", firstAddress.House);
            Assert.Equal(57.609197, firstAddress.Latitude);
            Assert.Equal(39.8040603, firstAddress.Longitude);
        }

        [Fact]
        public async Task GeocodeSettlement()
        {
            var result = await Geocoder.Geocode("Ярославская область Октябрьский 12", 1);
            var firstAddress = result.FirstOrDefault();

            Assert.Equal("Россия", firstAddress.Country);
            Assert.Equal("Ярославская обл", firstAddress.Province);
            Assert.Equal("Рыбинский р-н", firstAddress.Area);
            Assert.Equal("поселок Октябрьский", firstAddress.Locality);
            Assert.Null(firstAddress.Street);
            Assert.Equal("12", firstAddress.House);
            Assert.Equal(57.9847437, firstAddress.Latitude);
            Assert.Equal(39.110142, firstAddress.Longitude);
        }
    }
}
