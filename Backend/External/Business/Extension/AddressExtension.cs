using Common.Models.Geolocation;
using GoogleApi.Entities.Common;

namespace TransportSystems.Backend.External.Business.Extension
{
    public static class CoordinateExtension
    {
        public static Location ToLocation(this Coordinate coordinate)
        {
            return new Location(coordinate.Latitude, coordinate.Longitude);
        }
    }
}