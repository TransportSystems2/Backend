﻿using DotNetDistance;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using TransportSystems.Backend.External.Interfaces.Services.Direction;
using TransportSystems.Backend.External.Models.Routing;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationRouteService : ApplicationTransactionService, IApplicationRouteService
    {
        public ApplicationRouteService(
            ITransactionService transactionService,
            IDirectionService directionService,
            IRouteService domainRouteService,
            IRouteLegService domainRouteLegService,
            IApplicationAddressService addressService)
            : base(transactionService)
        {
            DirectionService = directionService;
            DomainRouteService = domainRouteService;
            DomainRouteLegService = domainRouteLegService;
            AddressService = addressService;
        }

        protected IDirectionService DirectionService { get; }

        protected IRouteService DomainRouteService { get; }

        protected IRouteLegService DomainRouteLegService { get; }

        protected IApplicationAddressService AddressService { get; }

        public async Task<Route> CreateDomainRoute(RouteAM route)
        {
            var domainRoute = await DomainRouteService.Create(route.Comment);

            foreach (var leg in route.Legs)
            {
                var startAddressKind = AddressKind.Waypoint;
                var endAddressKind  = AddressKind.Waypoint;

                if (leg.Kind.Equals(RouteLegKind.Feed))
                {
                    startAddressKind = AddressKind.Market;
                }

                if (leg.Kind.Equals(RouteLegKind.WayBack))
                {
                    endAddressKind = AddressKind.Market;
                }

                var startDomainAddress = await AddressService.GetOrCreateDomainAddress(startAddressKind, leg.StartAddress);
                var endDomainAddress = await AddressService.GetOrCreateDomainAddress(endAddressKind, leg.EndAddress);

                var routeLeg = await DomainRouteLegService.Create(
                    domainRoute.Id,
                    leg.Kind,
                    startDomainAddress.Id,
                    endDomainAddress.Id,
                    leg.Duration,
                    leg.Distance);
            }

            return domainRoute;
        }

        public async Task<RouteAM> GetRoute(int routeId)
        {
            var domainRoute = await DomainRouteService.Get(routeId);
            if (domainRoute == null)
            {
                throw new ArgumentException($"RouteId:{routeId} is null", "Route");
            }

            var result = new RouteAM { Comment = domainRoute.Comment };
            
            var domainLegs = await DomainRouteLegService.GetByRoute(routeId);
            foreach(var domainLeg in domainLegs)
            {
                var leg = await GetRouteLeg(domainLeg);
                result.Legs.Add(leg);
            }

            return result;
        }

        public virtual async Task<RouteLegAM> GetRouteLeg(RouteLeg source)
        {
            var result = new RouteLegAM
            {
                StartAddress = await AddressService.GetAddress(source.StartAddressId),
                EndAddress = await AddressService.GetAddress(source.EndAddressId),
                Duration = source.Duration,
                Distance = source.Distance,
                Kind = source.Kind
            };
            
            return result;
        }

        public async Task<ICollection<RouteAM>> FindRoutes(ICollection<AddressAM> rootAddresses, WaypointsAM waypoints)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            var result = new ConcurrentBag<RouteAM>();

            await rootAddresses.ParallelForEachAsync(
                async rootAddress =>
                {
                    try
                    {
                        var route = await FindRoute(rootAddress, waypoints);
                        if (route.Legs.Any())
                        {
                            result.Add(route);
                        }
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                }
            );

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return result.ToList();
        }

        public async Task<RouteAM> FindRoute(AddressAM rootAddress, WaypointsAM waypoints)
        {
            var rootCoordinate = rootAddress.ToCoordinate();
            var waypointsCoordinate = waypoints.Points.Select(p => p.ToCoordinate());
            var externalRoute = await DirectionService.GetRoute(rootCoordinate, rootCoordinate, waypointsCoordinate);

            var result = await FromExternalRoute(rootAddress, rootAddress, waypoints, externalRoute);

            return result;
        }

        public async Task<RouteAM> FromExternalRoute(AddressAM origin, AddressAM destination, WaypointsAM waypoints, RouteEM externalRoute)
        {
            var result = new RouteAM { Comment = waypoints.Comment };

            var routePoints = new List<AddressAM> { origin, destination };
            routePoints.AddRange(waypoints.Points);
            routePoints = routePoints.Distinct().ToList();

            foreach(var externalLeg in externalRoute.Legs)
            {
                var routeLegKind = RouteLegKind.Transportation;
                if (externalLeg.Equals(externalRoute.Legs.First()))
                {
                    routeLegKind = RouteLegKind.Feed;
                }

                if (externalLeg.Equals(externalRoute.Legs.Last()))
                {
                    routeLegKind = RouteLegKind.WayBack;
                }

                var startAddress = await AddressService.GetNearestAddress(externalLeg.StartCoordinate, routePoints);
                startAddress.AdjustedLatitude = externalLeg.StartCoordinate.Latitude;
                startAddress.AdjustedLongitude = externalLeg.StartCoordinate.Longitude;

                var endAddress = await AddressService.GetNearestAddress(externalLeg.EndCoordinate, routePoints);
                endAddress.AdjustedLatitude = externalLeg.EndCoordinate.Latitude;
                endAddress.AdjustedLongitude = externalLeg.EndCoordinate.Longitude;

                var leg = new RouteLegAM
                {
                    Kind = routeLegKind,
                    StartAddress = startAddress,
                    EndAddress = endAddress,
                    Distance = externalLeg.Distance,
                    Duration = externalLeg.Duration
                };

                result.Legs.Add(leg);
            }

            return result;
        }

        public async Task<string> GetShortTitle(int routeId)
        {
            var route = await DomainRouteService.Get(routeId);
            if (route == null)
            {
                throw new EntityNotFoundException($"RouteId:{route} not found", "Id");
            }

            var transportationsLegs = await DomainRouteLegService.GetByRoute(routeId, RouteLegKind.Transportation);

            var shortTitles = new List<string>();

            foreach (var leg in transportationsLegs)
            {
                var startShortTitle = await AddressService.GetShortTitle(leg.StartAddressId);
                var endShortTitle = await AddressService.GetShortTitle(leg.EndAddressId);

                shortTitles.Add(startShortTitle);
                shortTitles.Add(endShortTitle);
            }

            var normalizedShortTitles = GetNormalizedShortTitles(shortTitles);

            return string.Join(" - ", normalizedShortTitles);
        }

        public async Task<Distance> GetTotalDistance(int routeId)
        {
            return await DomainRouteLegService.GetDistance(routeId);
        }

        public Distance GetTotalDistance(RouteAM route)
        {
            return route.Legs.Select(l => l.Distance).Sum();
        }

        public AddressAM GetRootAddress(RouteAM route)
        {
            return GetLeg(route, RouteLegKind.Feed)?.StartAddress;
        }

        public Distance GetFeedDistance(RouteAM route)
        {
            var feedLeg = GetLeg(route, RouteLegKind.Feed);
            var result = feedLeg != null ? feedLeg.Distance : Distance.FromMeters(0);

            return result;
        }

        public Task<TimeSpan> GetFeedDuration(RouteAM route)
        {
            var feedLeg = GetLeg(route, RouteLegKind.Feed);

            return Task.FromResult(feedLeg.Duration);
        }

        public RouteLegAM GetLeg(RouteAM route, RouteLegKind legKind)
        {
            return route.Legs.FirstOrDefault(l => l.Kind.Equals(legKind));
        }

        private List<string> GetNormalizedShortTitles(List<string> shortTitles)
        {
            var result = new List<string>();

            foreach (var shortTitle in shortTitles)
            {
                var lastNormalizedShortTitle = result.LastOrDefault();

                if (string.IsNullOrEmpty(lastNormalizedShortTitle) || !lastNormalizedShortTitle.Equals(shortTitle))
                {
                    result.Add(shortTitle);
                }
            }

            return result;
        }
    }
}