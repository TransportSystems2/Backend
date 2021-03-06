﻿using DotNetDistance;
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

        Task<RouteAM> GetRoute(int routeId);

        Task<RouteLegAM> GetRouteLeg(RouteLeg source);

        Task<RouteAM> FindRoute(AddressAM rootAddress, WaypointsAM waypoints);

        Task<RouteAM> FromExternalRoute(AddressAM origin, AddressAM destination, WaypointsAM waypoints, RouteEM externalRoute);

        Task<ICollection<RouteAM>> FindRoutes(ICollection<AddressAM> rootAddresses, WaypointsAM waypoints);

        AddressAM GetRootAddress(RouteAM route);

        Distance GetTotalDistance(RouteAM route);

        Task<Distance> GetTotalDistance(int routeId);

        Distance GetFeedDistance(RouteAM route);

        Task<TimeSpan> GetFeedDuration(RouteAM route);
    }
}