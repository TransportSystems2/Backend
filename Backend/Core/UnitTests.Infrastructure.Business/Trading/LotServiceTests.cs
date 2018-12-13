using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;

using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;
using TransportSystems.Backend.Core.Infrastructure.Business.Trading;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Trading
{
    public class LotServiceTestSuite
    {
        public LotServiceTestSuite()
        {
            LotRepositoryMock = new Mock<ILotRepository>();
            OrderStateServiceMock = new Mock<IOrderStateService>();
            DispatcherServiceMock = new Mock<IDispatcherService>();

            LotService = new LotService(
                LotRepositoryMock.Object,
                OrderStateServiceMock.Object,
                DispatcherServiceMock.Object
                );
        }

        public ILotService LotService { get; }

        public Mock<ILotRepository> LotRepositoryMock { get; }

        public Mock<IOrderStateService> OrderStateServiceMock { get; }

        public Mock<IDispatcherService> DispatcherServiceMock { get; }
    }

    public class LotServiceTests
    {
        [Fact]
        public async void GetNewLotResultNewLot()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;

            var orderState = new OrderState { Id = commonId++, OrderId = orderId, Status = OrderStatus.ReadyForTrade };

            suite.LotRepositoryMock
                .Setup(m => m.GetByOrder(orderId))
                .Returns(Task.FromResult<Lot>(null));

            suite.OrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(orderState);

            suite.OrderStateServiceMock
                .Setup(m => m.IsExist(orderId))
                .ReturnsAsync(true);

            var lot = await suite.LotService.GetByOrder(orderId);

            Assert.Equal(LotStatus.New, lot.Status);
            Assert.Equal(orderId, lot.OrderId);
        }

        [Fact]
        public async void GetExistLotResultGotLot()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;
            var lot = new Lot { Id = commonId++, OrderId = orderId, Status = LotStatus.Traded };
            var orderState = new OrderState { Id = commonId++, OrderId = orderId, Status = OrderStatus.SentToTrading };

            suite.OrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(orderState);

            suite.LotRepositoryMock
                .Setup(m => m.GetByOrder(orderId))
                .ReturnsAsync(lot);

            var result = await suite.LotService.GetByOrder(orderId);

            suite.LotRepositoryMock
                .Verify(m => m.GetByOrder(orderId));
        }

        [Fact]
        public async void GetLotByNotExistOrderResultOrderNotFoundException()
        {
            var suite = new LotServiceTestSuite();

            suite.OrderStateServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Order", () => suite.LotService.GetByOrder(1));
        }

        [Fact]
        public async void TradeNewLotResultTradedStatus()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;
            var orderState = new OrderState { Id = commonId++, OrderId = orderId, Status = OrderStatus.ReadyForTrade };
            var lot = new Lot { Id = 1, OrderId = orderId, Status = LotStatus.New };

            suite.OrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(orderState);
            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);

            await suite.LotService.Trade(lot.Id);
            Assert.Equal(LotStatus.Traded, lot.Status);
        }

        [Fact]
        public async void TradeExpiredLotResultTradedStatus()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;
            var orderState = new OrderState { Id = commonId++, OrderId = orderId, Status = OrderStatus.SentToTrading };
            var lot = new Lot { Id = 1, OrderId = orderId, Status = LotStatus.Expired };

            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.OrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(orderState);

            await suite.LotService.Trade(lot.Id);
            Assert.Equal(LotStatus.Traded, lot.Status);
        }

        [Fact]
        public async void TradeAlreadyTradedLotResultAlreadyLotStatusException()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;
            var lot = new Lot { Id = commonId++, OrderId = orderId, Status = LotStatus.Traded };

            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.OrderStateServiceMock
                .Setup(m => m.IsExist(orderId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<LotStatusException>(() => suite.LotService.Trade(lot.Id));
        }

        [Fact]
        public async void TradeCanceledLotResultLotStatusException()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;
            var lot = new Lot { Id = commonId++, OrderId = orderId, Status = LotStatus.Canceled };
            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.OrderStateServiceMock
                .Setup(m => m.IsExist(orderId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<LotStatusException>(() => suite.LotService.Trade(lot.Id));
        }

        [Fact]
        public async void TradeNullLotResultLotNotFoundException()
        {
            var suite = new LotServiceTestSuite();

            await Assert.ThrowsAsync<EntityNotFoundException>("Lot", () => suite.LotService.Trade(1));
        }

        [Fact]
        public async void CancelLotResultCanceledStatus()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var lot = new Lot { Id = commonId++, OrderId = commonId++ };

            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);

            await suite.LotService.Cancel(lot.Id);

            Assert.Equal(LotStatus.Canceled, lot.Status);
        }

        [Fact]
        public async void CancelAlreadyCanceledLotResultLotStatusException()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var lot = new Lot { Id = commonId++, OrderId = commonId++, Status = LotStatus.Canceled };
            suite.LotRepositoryMock.Setup(m => m.Get(It.IsAny<int>())).ReturnsAsync(lot);

            await Assert.ThrowsAsync<LotStatusException>(() => suite.LotService.Cancel(lot.Id));
        }

        [Fact]
        public async void CancelNotExistLotResultLotNotFoundException()
        {
            var suite = new LotServiceTestSuite();
            suite.LotRepositoryMock
                .Setup(m => m.IsExist(0))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Lot", () => suite.LotService.Cancel(0));
        }

        [Fact]
        public async void WinLotResultWonStatus()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;
            var orderState = new OrderState { Id = commonId++, OrderId = orderId, Status = OrderStatus.SentToTrading };
            var lot = new Lot { Id = commonId++, OrderId = orderId, Status = LotStatus.Traded };
            var dispatcher = new Dispatcher { Id = commonId++ };

            suite.OrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(orderState);
            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);

            await suite.LotService.Win(lot.Id, dispatcher.Id);

            Assert.Equal(LotStatus.Won, lot.Status);
        }

        [Fact]
        public async void WinDispatcherLotResultSameWinner()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();

            var orderId = commonId++;
            var orderState = new OrderState { Id = commonId++, OrderId = orderId, Status = OrderStatus.SentToTrading };
            var lot = new Lot { Id = commonId++, OrderId = orderId, Status = LotStatus.Traded };
            var dispatcher = new Dispatcher { Id = commonId++ };

            suite.OrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(orderState);
            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);

            await suite.LotService.Win(lot.Id, dispatcher.Id);

            Assert.Equal(dispatcher.Id, lot.WinnerDispatcherId);
        }

        [Fact]
        public async void WinLotWithoutTradedOrderStatusResultLotStatusException()
        {
            var suite = new LotServiceTestSuite();

            var lot = new Lot { Id = 1, OrderId = 1 };
            var dispatcher = new Dispatcher { Id = 1 };

            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<LotStatusException>(() => suite.LotService.Win(lot.Id, dispatcher.Id));
        }

        [Fact]
        public async void WinNotExistLotResultLotStatusException()
        {
            var suite = new LotServiceTestSuite();

            var dispatcher = new Dispatcher { Id = 1 };
            suite.LotRepositoryMock
                .Setup(m => m.Get(0))
                .Returns(Task.FromResult<Lot>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>("Lot", () => suite.LotService.Win(0, dispatcher.Id));
        }

        [Fact]
        public async void WinLotWithoutDispatcherResultArgumentNullException()
        {
            var commonId = 1;
            var suite = new LotServiceTestSuite();
            var orderId = commonId++;
            var orderState = new OrderState { Id = commonId++, OrderId = orderId, Status = OrderStatus.SentToTrading };
            var lot = new Lot { Id = commonId++, OrderId = orderId, Status = LotStatus.Traded };

            suite.OrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(orderState);
            suite.LotRepositoryMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(0))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Dispatcher", () => suite.LotService.Win(lot.Id, 0));
        }

        [Fact]
        public async Task GetLotsByStatusWhereStatusIsTrade()
        {
            var suite = new LotServiceTestSuite();

            var status = LotStatus.Traded;
            var result = await suite.LotService.GetByStatus(status);

            suite.LotRepositoryMock
                .Verify(m => m.GetByStatus(status));
        }
    }
}