using Common.Models.Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Direction;
using TransportSystems.Backend.External.Interfaces.Exceptions;
using TransportSystems.Backend.External.Interfaces.Services;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Routing;

namespace TransportSystems.Backend.External.Business.Direction
{
    public class DirectionService : IDirectionService
    {
        public DirectionService(IDirectionAccessor providerAccessor)
        {
            ProviderAccessor = providerAccessor;
        }

        protected IDirectionAccessor ProviderAccessor { get; }

        public ProviderKind[] DefaultProvidersKind { get => new[] { ProviderKind.Google }; }

        public async Task<RouteEM> GetRoute(Coordinate origin, Coordinate destination, IEnumerable<Coordinate> waypoints, params ProviderKind[] providersKind)
        {
            if (providersKind.Length == 0)
            {
                providersKind = DefaultProvidersKind;
            }

            foreach (var providerKind in providersKind)
            {
                var provider = ProviderAccessor.GetProvider(providerKind);
                if (provider == null)
                {
                    continue;
                }

                var result = await provider.GetRoute(origin, destination, waypoints);
                if (result.Status == Status.Ok)
                {
                    return result;
                }
            }

            throw new DirectionException(Status.ZeroResults, "Route didn't found");
        }

        public Task<ICollection<Coordinate>> GetNearestCoordinates(Coordinate originCoordinate, IEnumerable<Coordinate> coordinates, int maxResultCount = 5)
        {
            maxResultCount = Math.Min(maxResultCount, coordinates.Count());

            return Task.FromResult((ICollection<Coordinate>)coordinates.OrderBy(
                coordinate => Geolocation.GeoCalculator.GetDistance(
                    originCoordinate.Latitude,
                    originCoordinate.Longitude,
                    coordinate.Latitude,
                    coordinate.Longitude))
                .ToList().GetRange(0, maxResultCount));
        }

        public async Task<Coordinate> GetNearestCoordinate(Coordinate originCoordinate, IEnumerable<Coordinate> coordinates)
        {
            var result = await GetNearestCoordinates(originCoordinate, coordinates, 1);

            return result.FirstOrDefault();
        }

        public Task<CoordinateBounds> GetCoordinateBounds(Coordinate originCoordinate, double distance)
        {
            var boundaries = new Geolocation.CoordinateBoundaries(originCoordinate.Latitude, originCoordinate.Longitude, distance);
            var result = new CoordinateBounds
            {
                Latitude = boundaries.Latitude,
                Longitude = boundaries.Longitude,
                MinLatitude = boundaries.MinLatitude,
                MinLongitude = boundaries.MinLongitude,
                MaxLatitude = boundaries.MaxLatitude,
                MaxLongitude = boundaries.MaxLongitude
            };

            return Task.FromResult(result);
        }

    }
}