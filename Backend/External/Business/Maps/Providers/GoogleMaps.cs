using System;
using System.Threading.Tasks;
using Common.Models;
using Common.Models.Units;
using GoogleApi.Entities.Maps.TimeZone.Request;
using TransportSystems.Backend.External.Interfaces.Exceptions;
using TransportSystems.Backend.External.Interfaces.Maps;
using TransportSystems.Backend.External.Models.Enums;
using Status = TransportSystems.Backend.External.Models.Enums.Status;

namespace TransportSystems.Backend.External.Business.Maps.Providers
{
    public class GoogleMaps : IMaps
    {
        public ProviderKind Kind => ProviderKind.Google;

        public async Task<TimeBelt> GetTimeBelt(Coordinate coordinate)
        {
            var timeZoneRequest = new TimeZoneRequest
            {
                Key = GoogleConfig.ApiKey,
                Language = GoogleApi.Entities.Common.Enums.Language.Russian,
                Location = new GoogleApi.Entities.Common.Location(coordinate.Latitude, coordinate.Longitude)
            };

            var timeZoneResponse = await GoogleApi.GoogleMaps.TimeZone.QueryAsync(timeZoneRequest);

            var status = (Status)timeZoneResponse.Status;
            if (status != Status.Ok)
            {
                throw new ResponseStatusException(status);
            }

            return new TimeBelt
            {
                Id = timeZoneResponse.TimeZoneId,
                Name = timeZoneResponse.TimeZoneName,
                OffSet = TimeSpan.FromSeconds(timeZoneResponse.OffSet),
                RawOffset = TimeSpan.FromSeconds(timeZoneResponse.RawOffSet)
            };
        }
    }
}