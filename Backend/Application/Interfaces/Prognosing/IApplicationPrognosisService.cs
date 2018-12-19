using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.Application.Interfaces.Prognosing
{
    public interface IApplicationPrognosisService : IApplicationBaseService
    {
        Task<TimeSpan> GetAvgTradingTime(RouteAM route, CargoAM cargo, BasketAM basket);

        Task<TimeSpan> GetAvgPreparationDriverTime(RouteAM route, CargoAM cargo, BasketAM basket);

        Task<TimeSpan> GetAvgDeliveryTime(RouteAM route, CargoAM cargo, BasketAM basket);
    }
}