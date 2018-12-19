using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Infrastructure.Business;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Ordering
{
    public class OrderStateServiceTestSuite
    {
        public OrderStateServiceTestSuite()
        {
            OrderStateRepositoryMock = new Mock<IOrderStateRepository>();
            OrderServiceMock = new Mock<IOrderService>();
            ModeratorServiceMock = new Mock<IModeratorService>();
            DispatcherServiceMock = new Mock<IDispatcherService>();
            DriverServiceMock = new Mock<IDriverService>();

            OrderStateService = new OrderStateService(
                OrderStateRepositoryMock.Object,
                OrderServiceMock.Object,
                ModeratorServiceMock.Object,
                DispatcherServiceMock.Object,
                DriverServiceMock.Object);
        }

        public IOrderStateService OrderStateService { get; }

        public Mock<IOrderStateRepository> OrderStateRepositoryMock { get; }

        public Mock<IOrderService> OrderServiceMock { get; }

        public Mock<IModeratorService> ModeratorServiceMock { get; }

        public Mock<IDispatcherService> DispatcherServiceMock { get; }

        public Mock<IDriverService> DriverServiceMock { get; }
    }

    public class OrderStateServiceTests
    {
        /*
        [Fact]
        public async void GetCurrentState()
        {
            var suite = new OrderStateServiceTestSuite();

            var orderId = 1;

            suite.OrderServiceMock
                .Setup(m => m.IsExistEntity(orderId))
                .ReturnsAsync(true);

            var currentState = await suite.OrderStateService.GetCurrentState(orderId);

            suite.OrderStateRepositoryMock
                .Verify(m => m.GetCurrentState(orderId));
        }
        */

        /*
        [Fact]
        public async void GetCurrentStateForNotExistingOrder()
        {
            var suite = new OrderStateServiceTestSuite();
            suite.OrderServiceMock
                .Setup(m => m.IsExistEntity(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Order", () => suite.OrderStateService.GetCurrentState(1));
        }
        */

        [Fact]
        public async void GetStatesWithNewCurentStatus()
        {
            var suite = new OrderStateServiceTestSuite();
            var status = OrderStatus.New;

            var newStates = await suite.OrderStateService.GetByCurrentStatus(status);
            suite.OrderStateRepositoryMock
                .Verify(m => m.GetStatesByCurrentStatus(status));
        }

        [Fact]
        public async void GetAcceptedStatesCount()
        {
            var suite = new OrderStateServiceTestSuite();
            var status = OrderStatus.Accepted;

            var count = await suite.OrderStateService.GetCountByCurrentStatus(status);

            suite.OrderStateRepositoryMock
                .Verify(m => m.GetCountStatesByCurrentStatus(status));
        }

        [Fact]
        public async void Accept()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };
            var moderator = new Moderator { Id = commonId++ };

            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.Accept(order.Id, moderator.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.Accepted))));
            suite.OrderServiceMock
                .Verify(m => m.AssignModerator(order.Id, moderator.Id));
        }

        [Fact]
        public async void AcceptWhenOrderDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };
            var moderator = new Moderator { Id = commonId++ };

            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .Returns(Task.FromResult<Order>(null));
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Order",
                () => suite.OrderStateService.Accept(order.Id, moderator.Id));
        }

        [Fact]
        public async void AcceptWhenModeratorDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };
            var moderator = new Moderator { Id = commonId++ };

            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Moderator", 
                () => suite.OrderStateService.Accept(order.Id, moderator.Id));
        }

        [Fact]
        public async void AcceptWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };
            var moderator = new Moderator { Id = commonId++ };

            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(
                () => suite.OrderStateService.Accept(order.Id, moderator.Id));
        }

        [Fact]
        public async void ReadyToTrade()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var moderator = new Moderator { Id = commonId++ };
            var order = new Order { Id = commonId++, ModeratorId = moderator.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.Accepted };

            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.ReadyToTrade(order.Id, moderator.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.ReadyForTrade))));
        }

        [Fact]
        public async void ReadyToTradeWhenModeratorIsOther()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var moderator = new Moderator { Id = commonId++ };
            var order = new Order { Id = commonId++, ModeratorId = -1 };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.Accepted };

            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.ReadyToTrade(order.Id, moderator.Id));
        }

        [Fact]
        public async void Trade()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.ReadyForTrade };

            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.Trade(order.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.SentToTrading))));
        }

        [Fact]
        public async void TradeWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.Accepted };

            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.Trade(order.Id));
        }

        [Fact]
        public async void AssignOrderToDispatcher()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var dispatcher = new Dispatcher { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.Accepted };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.AssignToDispatcher(order.Id, dispatcher.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.AssignedDispatcher))));
            suite.OrderServiceMock
                .Verify(m => m.AssignDispatcher(order.Id, dispatcher.Id));
        }

        [Fact]
        public async void AssignToDispatcherWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var dispatcher = new Dispatcher { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.AssignToDispatcher(order.Id, dispatcher.Id));
        }

        [Fact]
        public async void AssignToDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var dispatcher = new Dispatcher { Id = commonId++ };
            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DispatcherId = dispatcher.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.AssignToDriver(order.Id, dispatcher.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.AssignedDriver))));
            suite.OrderServiceMock
                .Verify(m => m.AssignDriver(order.Id, driver.Id));
        }

        [Fact]
        public async void AssignToDriverWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var dispatcher = new Dispatcher { Id = commonId++ };
            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DispatcherId = dispatcher.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.AssignToDriver(order.Id, dispatcher.Id, driver.Id));
        }

        [Fact]
        public async void AssignToDriverWhenAnotherDispatcher()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var orderDispatcher = new Dispatcher { Id = commonId++ };
            var anotherDispatcherId = commonId++;
            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DispatcherId = orderDispatcher.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(true);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.AssignToDriver(order.Id, anotherDispatcherId, driver.Id));
        }

        [Fact]
        public async void AssignToDriverWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var dispatcher = new Dispatcher { Id = commonId++ };
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++, DispatcherId = dispatcher.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Driver",
                () => suite.OrderStateService.AssignToDriver(order.Id, dispatcher.Id, notExistingDriverId));
        }

        [Fact]
        public async void AssignToDriverWhenDispatcherDoesNotFound()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var notExistingDispatcher = commonId++;
            var order = new Order { Id = commonId++, DispatcherId = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(notExistingDispatcher))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>("Dispatcher", () => suite.OrderStateService.AssignToDriver(order.Id, notExistingDispatcher, driver.Id));
        }

        [Fact]
        public async void ConfirmByDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDriver };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.ConfirmByDriver(order.Id, driver.Id);
            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.ConfirmedByDriver))));
        }

        [Fact]
        public async void ConfirmWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var notExistingDriverId = commonId++; 
            var order = new Order { Id = commonId++, DriverId = notExistingDriverId };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.OrderStateService.ConfirmByDriver(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ConfirmWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = commonId++ };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.ConfirmByDriver(order.Id, driver.Id));
        }

        [Fact]
        public async void ConfirmWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.ConfirmByDriver(order.Id, driver.Id));
        }

        [Fact]
        public async void GoToCustomerResultWentToCustomer()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.ConfirmedByDriver };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.GoToCustomer(order.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.WentToCustomer))));
        }

        [Fact]
        public async void GoToCustomerWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++, DriverId = notExistingDriverId };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.OrderStateService.GoToCustomer(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void GoToCustomerWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = commonId++ };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.GoToCustomer(order.Id, driver.Id));
        }

        [Fact]
        public async void GoToCustomerWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDriver };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.GoToCustomer(order.Id, driver.Id));
        }

        [Fact]
        public async void ArriveAtLoading()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.WentToCustomer };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.ArriveAtLoadingPlace(order.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.ArrivedAtLoadingPlace))));
        }

        [Fact]
        public async void ArriveAtLoadingWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++, DriverId = notExistingDriverId };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.OrderStateService.ArriveAtLoadingPlace(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ArriveAtLoadingWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = commonId++ };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.ArriveAtLoadingPlace(order.Id, driver.Id));
        }

        [Fact]
        public async void ArriveAtLoadingPlaceWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.ConfirmedByDriver };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.ArriveAtLoadingPlace(order.Id, driver.Id));
        }

        [Fact]
        public async void LoadTheVehicleResultVehicleIsLoaded()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.ArrivedAtLoadingPlace };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.LoadTheVehicle(order.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.VehicleIsLoaded))));
        }

        [Fact]
        public async void LoadTheVehicleWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++, DriverId = notExistingDriverId };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.OrderStateService.LoadTheVehicle(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void LoadTheVehicleWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = commonId++ };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.LoadTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void LoadTheVehicleWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.WentToCustomer };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.LoadTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void DeliverTheVehicleResultVehicleIsDelivered()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.VehicleIsLoaded };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.DeliverTheVehicle(order.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.VehicleIsDelivered))));
        }

        [Fact]
        public async void DeliverTheVehicleWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++, DriverId = notExistingDriverId };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.OrderStateService.DeliverTheVehicle(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void DeliverTheVehicleWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = commonId++ };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.DeliverTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void DeliverTheVehicleWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.ArrivedAtLoadingPlace };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.DeliverTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void ReceivePaymentResultPaymentIsReceived()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.VehicleIsDelivered };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.ReceivePayment(order.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.PaymentIsReceived))));
        }

        [Fact]
        public async void ReceivePaymentWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++, DriverId = notExistingDriverId };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.OrderStateService.ReceivePayment(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ReceivePaymentWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = commonId++ };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.OrderStateService.ReceivePayment(order.Id, driver.Id));
        }

        [Fact]
        public async void ReceivePaymentWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.VehicleIsLoaded };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.ReceivePayment(order.Id, driver.Id));
        }

        [Fact]
        public async void CompleteOrder()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.PaymentIsReceived };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.Complete(order.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.Completed))));
        }

        [Fact]
        public async void Cancel()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.WentToCustomer };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.OrderStateService.Cancel(order.Id, driver.Id);

            suite.OrderStateRepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.AssignedDispatcher))));
        }

        [Fact]
        public async void CompleteWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++, DriverId = driver.Id };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.OrderStateRepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.OrderStateService.Complete(order.Id, driver.Id));
        }
    }
}