using Common.Models.Geolocation;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Models.Enums;
using TransportSystems.Backend.External.Models.Routing;

namespace TransportSystems.Backend.External.Interfaces.Services.Direction
{
    public interface IDirectionService
    {
        ProviderKind[] DefaultProvidersKind { get; }

        Task<RouteEM> GetRoute(Coordinate origin, Coordinate destination, IEnumerable<Coordinate> waypoints, params ProviderKind[] providersKind);

        Task<Coordinate> GetNearestCoordinate(Coordinate originCoordinate, IEnumerable<Coordinate> coordinates);

        Task<ICollection<Coordinate>> GetNearestCoordinates(Coordinate originCoordinate, IEnumerable<Coordinate> coordinates, int maxResultCount = 5);

        Task<CoordinateBounds> GetCoordinateBounds(Coordinate originCoordinate, double distance);
    }
}