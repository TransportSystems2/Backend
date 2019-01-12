using Common.Models.Units;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Interfaces.Provider;
using TransportSystems.Backend.External.Models.Routing;

namespace TransportSystems.Backend.External.Interfaces.Direction
{
    public interface IDirection : IProvider
    {
        Task<RouteEM> GetRoute(Coordinate origin, Coordinate destination, IEnumerable<Coordinate> waypoints);
    }
}