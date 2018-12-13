using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Notification;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Notification;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Trading
{
    public class MarketService : BaseService, IMarketService
    {
        public MarketService(
            ILotService lotService,
            IOrderService orderService,
            IOrderStateService orderStateService,
            INotificationService notificationService,
            IDispatcherService dispatcherService)
        {
            LotService = lotService;
            OrderService = orderService;
            OrderStateService = orderStateService;
            NotificationService = notificationService;
            DispatcherService = dispatcherService;
        }

        protected ILotService LotService { get; }

        protected IOrderService OrderService { get; }

        protected IOrderStateService OrderStateService { get; }

        protected INotificationService NotificationService { get; }

        protected IDispatcherService DispatcherService { get; }

        public async Task LaunchTrading()
        {
            var ordersState = await OrderStateService.GetByCurrentStatus(OrderStatus.ReadyForTrade);

            await Task.WhenAll(ordersState.Select(state => Trade(state.OrderId)));
        }

        public async Task Trade(int orderId)
        {
            var lot = await LotService.GetByOrder(orderId);
            await LotService.Trade(lot.Id);

            var order = await OrderService.Get(orderId);
            
            var dispatchers = await DispatcherService.GetByCompany(order.CompanyId);

            var notify = new Notify(
                dispatchers,
                "Новый заказ",
                "Параметры нового заказа",
                NotificationChanelKind.All);
            await NotificationService.Notice(notify);
        }
    }
}