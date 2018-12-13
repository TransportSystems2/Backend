using Common.Models.Geolocation;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Business.Geo
{
    public static class AddressExtension
    {
        public static Coordinate ToCoordinate(this AddressAM address)
        {
            var result = new Coordinate
            {
                Latitude = address.Latitude,
                Longitude = address.Longitude,
            };

            return result;
        }

        public static Coordinate ToCoordinate(this Address address)
        {
            var result = new Coordinate
            {
                Latitude = address.Latitude,
                Longitude = address.Longitude,
            };

            return result;
        }
    }
}