using System.Collections.Generic;
using Moq;

using TransportSystems.Backend.Core.Domain.Core.Notification;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Infrastructure.Business.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Notification;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

using Xunit;

namespace TransportSystems.Infrastructure.Business.Tests.Trading
{
    /*
    public class MarketServiceSuite
    {
        public MarketServiceSuite()
        {
            LotServiceMock = new Mock<ILotService>();
            OrderServiceMock = new Mock<IOrderService>();
            OrderStateServiceMock = new Mock<IOrderStateService>();
            NotificationServiceMock = new Mock<INotificationService>();
            DispatcherServiceMock = new Mock<IDispatcherService>();

            MarketService = new MarketService(
                LotServiceMock.Object,
                OrderServiceMock.Object,
                OrderStateServiceMock.Object,
                NotificationServiceMock.Object,
                DispatcherServiceMock.Object);
        }

        public Mock<ILotService> LotServiceMock { get; private set; }

        public Mock<IOrderService> OrderServiceMock { get; private set; }

        public Mock<IOrderStateService> OrderStateServiceMock { get; private set; }

        public Mock<INotificationService> NotificationServiceMock { get; private set; }

        public Mock<IDispatcherService> DispatcherServiceMock { get; private set; }

        public IMarketService MarketService { get; private set; }
    }

    public class MarketServiceTests
    {
        [Fact]
        public async void TradeOrderResultStartOfTradingOnLotServiceAndNotifytingOfDispatchers()
        {
            var suite = new MarketServiceSuite();
            var commonId = 1;

            var orderState = new OrderState { Id = commonId++, OrderId = commonId++, CompanyId = commonId };
            var lot = new Lot { Id = commonId, OrderId = orderState.OrderId };
            var dispatchers = new List<Dispatcher>
            {
                new Dispatcher { Id = commonId++ },
                new Dispatcher { Id = commonId++ }
            };

            suite.LotServiceMock
                .Setup(m => m.GetByOrder(orderState.OrderId))
                .ReturnsAsync(lot);
            suite.OrderStateServiceMock
                .Setup(m => m.Get(orderState.Id))
                .ReturnsAsync(orderState);
            suite.DispatcherServiceMock
                .Setup(m => m.GetByCompany(orderState.CompanyId))
                .ReturnsAsync(dispatchers);

            await suite.MarketService.Trade(orderState.Id);

            suite.LotServiceMock
                .Verify(m => m.Trade(lot.Id), Times.Once);
            suite.NotificationServiceMock
                .Verify(m => m.Notice(It.IsAny<Notify>()), Times.Once);
        }
    }
    */
}