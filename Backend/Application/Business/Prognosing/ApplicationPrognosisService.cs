using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Prognosing;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.Application.Business.Prognosing
{
    public class ApplicationPrognosisService : ApplicationBaseService, IApplicationPrognosisService
    {
        public static TimeSpan DefaultAvgPreparationDeiverTime = TimeSpan.FromMinutes(10);
        public static TimeSpan DefaultAvgTradingTime = TimeSpan.FromMinutes(7);

        public ApplicationPrognosisService(
            IApplicationRouteService routeService)
        {
            RouteService = routeService;
        }

        protected IApplicationRouteService RouteService { get; }

        public Task<TimeSpan> GetAvgPreparationDriverTime(RouteAM route, CargoAM cargo, BasketAM basket)
        {
            return Task.FromResult(DefaultAvgPreparationDeiverTime);
        }

        public Task<TimeSpan> GetAvgTradingTime(RouteAM route, CargoAM cargo, BasketAM basket)
        {
            return Task.FromResult(DefaultAvgTradingTime);
        }

        public async Task<TimeSpan> GetAvgDeliveryTime(RouteAM route, CargoAM cargo, BasketAM basket)
        {
            var feedDurationTime = await RouteService.GetFeedDuration(route);
            var avgPreparationDriverTime = await GetAvgPreparationDriverTime(route, cargo, basket);
            var avgTradingTime = await GetAvgTradingTime(route, cargo, basket);

            var result = new TimeSpan();
            result = result.Add(feedDurationTime);
            result = result.Add(avgPreparationDriverTime);
            result = result.Add(avgTradingTime);

            return result;
        }
    }
}