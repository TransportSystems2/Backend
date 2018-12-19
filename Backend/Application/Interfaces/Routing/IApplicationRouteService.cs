using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.External.Models.Routing;

namespace TransportSystems.Backend.Application.Interfaces.Routing
{
    public interface IApplicationRouteService : IApplicationTransactionService
    {
        Task<string> GetShortTitle(int routeId);

        Task<Route> CreateDomainRoute(RouteAM route);

        Task<RouteAM> GetRoute(AddressAM rootAddress, WaypointsAM waypoints);

        Task<RouteAM> FromExternalRoute(AddressAM origin, AddressAM destination, WaypointsAM waypoints, RouteEM externalRoute);

        Task<ICollection<RouteAM>> GetPossibleRoutes(ICollection<AddressAM> rootAddresses, WaypointsAM waypoints);

        AddressAM GetRootAddress(RouteAM route);

        int GetTotalDistance(RouteAM route);

        int GetFeedDistance(RouteAM route);

        Task<TimeSpan> GetFeedDuration(RouteAM route);
    }
}