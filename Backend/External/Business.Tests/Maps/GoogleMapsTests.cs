using Common.Models.Units;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Business.Maps.Providers;
using TransportSystems.Backend.External.Interfaces.Maps;
using Xunit;

namespace TransportSystems.Backend.External.Business.Tests.Maps
{
    public class GoogleMapsTests
    {
        public GoogleMapsTests()
        {
            Maps = new GoogleMaps();
        }

        protected IMaps Maps { get; }

        [Fact]
        public async Task GetMoscowTimeBelt()
        {
            var moscowCoordinate = new Coordinate
            {
                Latitude = 55.767898,
                Longitude = 37.620393
            };

            var result = await Maps.GetTimeBelt(moscowCoordinate);

            Assert.Equal("Europe/Moscow", result.Id);
            Assert.Equal("Москва, стандартное время", result.Name);
            Assert.Equal(0, result.OffSet.TotalHours);
            Assert.Equal(3, result.RawOffset.TotalHours);
        }
    }
}