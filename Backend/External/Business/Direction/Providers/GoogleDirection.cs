using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models.Units;
using DotNetDistance;
using GoogleApi;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Directions.Request;
using GoogleApi.Entities.Maps.Directions.Response;
using TransportSystems.Backend.External.Business.Extension;
using TransportSystems.Backend.External.Interfaces.Direction;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Routing;
using Status = TransportSystems.Backend.External.Models.Enums.Status;

namespace TransportSystems.Backend.External.Business.Direction.Providers
{
    public class GoogleDirection : IDirection
    {
        public ProviderKind Kind => ProviderKind.Google;

        public async Task<RouteEM> GetRoute(Coordinate origin, Coordinate destination, IEnumerable<Coordinate> waypoints)
        {
            var result = new RouteEM();

            var directionRequest = new DirectionsRequest
            {
                Key = GoogleConfig.ApiKey,
                Origin = origin.ToLocation(),
                Destination = destination.ToLocation(),
                Waypoints = waypoints.Select(p => p.ToLocation()).ToArray(),
                Language = Language.Russian,
                OptimizeWaypoints = false
            };

            var directionResponse = await GoogleMaps.Directions.QueryAsync(directionRequest);

            result.Status = (Status)directionResponse.Status;

            var firstRoute = directionResponse.Routes.FirstOrDefault();
            if (firstRoute != null)
            {
                foreach (var leg in firstRoute.Legs)
                {
                    result.Legs.Add(LegFromGoogleLeg(leg));
                }
            }

            return result;
        }

        protected LegEM LegFromGoogleLeg(Leg googleLeg)
        {
            var result = new LegEM
            {
                Distance = Distance.FromMeters(googleLeg.Distance.Value),
                Duration = TimeSpan.FromSeconds(googleLeg.Duration.Value),

                StartCoordinate = new Coordinate
                {
                    Latitude = googleLeg.StartLocation.Latitude,
                    Longitude = googleLeg.StartLocation.Longitude
                },

                EndCoordinate = new Coordinate
                {
                    Latitude = googleLeg.EndLocation.Latitude,
                    Longitude = googleLeg.EndLocation.Longitude
                }
            };

            return result;
        }
    }
}