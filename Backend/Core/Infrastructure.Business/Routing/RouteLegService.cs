using DotNetDistance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Interfaces.Routing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Routing
{
    public class RouteLegService : DomainService<RouteLeg>, IRouteLegService
    {
        public RouteLegService(
            IRouteLegRepository repository,
            IRouteService routeService,
            IAddressService addressService)
            : base(repository)
        {
            RouteService = routeService;
            AddressService = addressService;
        }

        protected new IRouteLegRepository Repository => (IRouteLegRepository)base.Repository;

        protected IRouteService RouteService { get; }

        protected IAddressService AddressService { get; }

        public async Task<RouteLeg> Create(
            int routeId,
            RouteLegKind kind,
            int startAddressId,
            int endAddressId,
            TimeSpan duration,
            Distance distance)
        {
            var leg = new RouteLeg
            {
                RouteId = routeId,
                Kind = kind,
                StartAddressId = startAddressId,
                EndAddressId = endAddressId,
                Duration = duration,
                Distance = distance
            };

            await Verify(leg);

            await Repository.Add(leg);
            await Repository.Save();

            return leg;
        }

        public async Task<ICollection<RouteLeg>> GetByRoute(int routeId, RouteLegKind kind)
        {
            if (!await RouteService.IsExist(routeId))
            {
                throw new EntityNotFoundException($"RouteId:{routeId} doesn't exist.", "Route");
            }

            return await Repository.GetByRoute(routeId, kind);
        }

        protected override async Task<bool> DoVerifyEntity(RouteLeg entity)
        {
            if (!await RouteService.IsExist(entity.RouteId))
            {
                throw new EntityNotFoundException($"RouteId:{entity.RouteId} doesn't exist.", "Route");
            }

            if (!await AddressService.IsExist(entity.StartAddressId))
            {
                throw new EntityNotFoundException($"StartAddressId:{entity.StartAddressId} doesn't exist.", "StartAddress");
            }

            if (!await AddressService.IsExist(entity.EndAddressId))
            {
                throw new EntityNotFoundException($"EndAddressId:{entity.EndAddressId} doesn't exist.", "EndAddress");
            }

            if (entity.Duration.Ticks <= 0)
            {
                throw new ArgumentException($"Duration can't be lower than or equal to Zero. Duration:{entity.Duration}", "Duration");
            }

            if (entity.Distance.ToMeters() <= 0)
            {
                throw new ArgumentException($"Distance can't be lower than or equal to Zero. Distance:{entity.Distance}", "Distance");
            }

            return true;
        }
    }
}