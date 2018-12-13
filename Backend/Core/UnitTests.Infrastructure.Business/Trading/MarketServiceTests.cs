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

namespace TransportSystems.UnitTests.Infrastructure.Business.Trading
{
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

            var order = new Order { Id = 1, CompanyId = 1 };
            var lot = new Lot { Id = 1, OrderId = order.Id };
            var dispatchers = new List<Dispatcher>
            {
                new Dispatcher { Id = 1 },
                new Dispatcher { Id = 2 }
            };

            suite.LotServiceMock
                .Setup(m => m.GetByOrder(order.Id))
                .ReturnsAsync(lot);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.DispatcherServiceMock
                .Setup(m => m.GetByCompany(order.CompanyId))
                .ReturnsAsync(dispatchers);

            await suite.MarketService.Trade(order.Id);

            suite.LotServiceMock
                .Verify(m => m.Trade(lot.Id), Times.Once);
            suite.NotificationServiceMock
                .Verify(m => m.Notice(It.IsAny<Notify>()), Times.Once);
        }
    }
}