using Moq;
using System;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Infrastructure.Business;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using Xunit;

namespace TransportSystems.Infrastructure.Business.Tests.Ordering
{
    public class OrderStateServiceTestSuite
    {
        public OrderStateServiceTestSuite()
        {
            RepositoryMock = new Mock<IOrderStateRepository>();
            OrderServiceMock = new Mock<IOrderService>();
            ModeratorServiceMock = new Mock<IModeratorService>();
            DispatcherServiceMock = new Mock<IDispatcherService>();
            DriverServiceMock = new Mock<IDriverService>();
            CustomerServiceMock = new Mock<ICustomerService>();
            CargoServiceMock = new Mock<ICargoService>();
            RouteServiceMock = new Mock<IRouteService>();
            BillServiceMock = new Mock<IBillService>();
            MarketServiceMock = new Mock<IMarketService>();

            Service = new OrderStateService(
                RepositoryMock.Object,
                OrderServiceMock.Object,
                ModeratorServiceMock.Object,
                DispatcherServiceMock.Object,
                DriverServiceMock.Object,
                CustomerServiceMock.Object,
                CargoServiceMock.Object,
                RouteServiceMock.Object,
                BillServiceMock.Object,
                MarketServiceMock.Object);
        }

        public IOrderStateService Service { get; }

        public Mock<IOrderStateRepository> RepositoryMock { get; }

        public Mock<IOrderService> OrderServiceMock { get; }

        public Mock<IModeratorService> ModeratorServiceMock { get; }

        public Mock<IDispatcherService> DispatcherServiceMock { get; }

        public Mock<IDriverService> DriverServiceMock { get; }

        public Mock<ICustomerService> CustomerServiceMock { get; }

        public Mock<ICargoService> CargoServiceMock { get; }

        public Mock<IRouteService> RouteServiceMock { get; }

        public Mock<IBillService> BillServiceMock { get; }

        public Mock<IMarketService> MarketServiceMock { get; }
    }

    public class OrderStateServiceTests
    {
        [Fact]
        public async void GetCurrentState()
        {
            var suite = new OrderStateServiceTestSuite();

            var orderId = 1;

            suite.OrderServiceMock
                .Setup(m => m.IsExist(orderId))
                .ReturnsAsync(true);

            var currentState = await suite.Service.GetCurrentState(orderId);

            suite.RepositoryMock
                .Verify(m => m.GetCurrentState(orderId));
        }

        [Fact]
        public async void GetCurrentStateForNotExistingOrder()
        {
            var suite = new OrderStateServiceTestSuite();
            suite.OrderServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Order", () => suite.Service.GetCurrentState(1));
        }

        [Fact]
        public async void GetStatesWithNewCurentStatus()
        {
            var suite = new OrderStateServiceTestSuite();
            var status = OrderStatus.New;

            var newStates = await suite.Service.GetByCurrentStatus(status);
            suite.RepositoryMock
                .Verify(m => m.GetStatesByCurrentStatus(status));
        }

        [Fact]
        public async void GetAcceptedStatesCount()
        {
            var suite = new OrderStateServiceTestSuite();
            var status = OrderStatus.Accepted;

            var count = await suite.Service.GetCountByCurrentStatus(status);

            suite.RepositoryMock
                .Verify(m => m.GetCountStatesByCurrentStatus(status));
        }

        [Fact]
        public async void New()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var orderId = commonId++;
            var timeOfDelivery = DateTime.Now + TimeSpan.FromMinutes(30);
            var customerId = commonId++;
            var cargoId = commonId++;
            var routeId = commonId++;
            var billId = commonId++;
            var market = new Market
            {
                Id = commonId++,
                CompanyId = commonId++
            };

            suite.OrderServiceMock
                .Setup(m => m.IsExist(orderId))
                .ReturnsAsync(true);
            suite.CustomerServiceMock
                .Setup(m => m.IsExist(customerId))
                .ReturnsAsync(true);
            suite.CargoServiceMock
                .Setup(m => m.IsExist(cargoId))
                .ReturnsAsync(true);
            suite.RouteServiceMock
                .Setup(m => m.IsExist(routeId))
                .ReturnsAsync(true);
            suite.BillServiceMock
                .Setup(m => m.IsExist(billId))
                .ReturnsAsync(true);
            suite.MarketServiceMock
                .Setup(m => m.Get(market.Id))
                .ReturnsAsync(market);

            await suite.Service.New(orderId, market.Id, timeOfDelivery, customerId, cargoId, routeId, billId);
            suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(orderId)
                    && s.MarketId.Equals(market.Id)
                    && s.TimeOfDelivery.Equals(timeOfDelivery)
                    && s.CustomerId.Equals(customerId)
                    && s.CargoId.Equals(cargoId)
                    && s.RouteId.Equals(routeId)
                    && s.BillId.Equals(billId)
                    && s.GenCompanyId.Equals(market.CompanyId))));
        }

        [Fact]
        public async void Accept()
        {
            var commonId = 1;
            var companyId = commonId++;
            var suite = new OrderStateServiceTestSuite();
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.New,
                GenCompanyId = companyId
            };
            var genDispatcher = new Dispatcher { Id = commonId++, CompanyId = companyId };

            suite.DispatcherServiceMock
                .Setup(m => m.Get(genDispatcher.Id))
                .ReturnsAsync(genDispatcher);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.Accept(order.Id, genDispatcher.Id);

            suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.Accepted)
                    && s.GenDispatcherId.Equals(genDispatcher.Id))));
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
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(false);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Order",
                () => suite.Service.Accept(order.Id, moderator.Id));
        }

        [Fact]
        public async void AcceptWhenGenDispatcerDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };
            var genDispatcher = new Dispatcher { Id = commonId++ };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(genDispatcher.Id))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "GenDispatcher", 
                () => suite.Service.Accept(order.Id, genDispatcher.Id));
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
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(
                () => suite.Service.Accept(order.Id, moderator.Id));
        }

        [Fact]
        public async void ReadyToTrade()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var genDispatcher = new Dispatcher { Id = commonId++ };
            var order = new Order { Id = commonId++};
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.Accepted,
                GenDispatcherId = genDispatcher.Id
            };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(genDispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.ReadyToTrade(order.Id, genDispatcher.Id);

            suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.ReadyForTrade))));
        }

        [Fact]
        public async void ReadyToTradeWhenDispatcherIsOther()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var genDispatcher = new Dispatcher { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.Accepted,
                GenDispatcherId = -1
            };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(genDispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.ReadyToTrade(order.Id, genDispatcher.Id));
        }

        [Fact]
        public async void Trade()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.ReadyForTrade };

            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.Trade(order.Id);

            suite.RepositoryMock
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
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.Trade(order.Id));
        }

        [Fact]
        public async void AssignOrderToSubDispatcher()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var subCompanyId = commonId++;
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.Accepted };

            suite.DispatcherServiceMock
                .Setup(m => m.Get(subDispatcher.Id))
                .ReturnsAsync(subDispatcher);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.AssignToSubDispatcher(order.Id, subDispatcher.Id);

            suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.AssignedDispatcher)
                    && s.SubDispatcherId.Equals(subDispatcher.Id)
                    && s.SubCompanyId.Equals(subCompanyId))));
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
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.AssignToSubDispatcher(order.Id, dispatcher.Id));
        }

        [Fact]
        public async void AssignToDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var subDispatcher = new Dispatcher { Id = commonId++ };
            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.AssignToDriver(order.Id, subDispatcher.Id, driver.Id);

            suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.AssignedDriver)
                    && s.DriverId.Equals(driver.Id))));
        }

        [Fact]
        public async void AssignToDriverWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var subDispatcher = new Dispatcher { Id = commonId++ };
            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.New,
                SubDispatcherId = subDispatcher.Id
            };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.AssignToDriver(order.Id, subDispatcher.Id, driver.Id));
        }

        [Fact]
        public async void AssignToDriverWhenAnotherDispatcher()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var orderSubDispatcher = new Dispatcher { Id = commonId++ };
            var anotherDispatcherId = commonId++;
            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = orderSubDispatcher.Id
            };

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(true);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.AssignToDriver(order.Id, anotherDispatcherId, driver.Id));
        }

        [Fact]
        public async void AssignToDriverWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var subDispatcher = new Dispatcher { Id = commonId++ };
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Driver",
                () => suite.Service.AssignToDriver(order.Id, subDispatcher.Id, notExistingDriverId));
        }

        [Fact]
        public async void AssignToDriverWhenDispatcherDoesNotFound()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var notExistingDispatcher = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = commonId++
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(notExistingDispatcher))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>("SubDispatcher", () => suite.Service.AssignToDriver(order.Id, notExistingDispatcher, driver.Id));
        }

        [Fact]
        public async void ConfirmByDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDriver,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.ConfirmByDriver(order.Id, driver.Id);
            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDriver
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.Service.ConfirmByDriver(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ConfirmWhenDriverIsAnother()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var anotherDriver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDriver,
                DriverId = driver.Id
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.ConfirmByDriver(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void ConfirmWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.ConfirmByDriver(order.Id, driver.Id));
        }

        [Fact]
        public async void GoToCustomerResultWentToCustomer()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ConfirmedByDriver,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.GoToCustomer(order.Id, driver.Id);

            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ConfirmedByDriver
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.Service.GoToCustomer(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void GoToCustomerWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var anotherDriver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ConfirmedByDriver,
                DriverId = driver.Id
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.GoToCustomer(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void GoToCustomerWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDriver,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.GoToCustomer(order.Id, driver.Id));
        }

        [Fact]
        public async void ArriveAtLoading()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.ArriveAtLoadingPlace(order.Id, driver.Id);

            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.Service.ArriveAtLoadingPlace(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ArriveAtLoadingWhenDriverIsAnother()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var anotherDriver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer,
                DriverId = driver.Id
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.ArriveAtLoadingPlace(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void ArriveAtLoadingPlaceWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ConfirmedByDriver,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.ArriveAtLoadingPlace(order.Id, driver.Id));
        }

        [Fact]
        public async void LoadTheVehicleResultVehicleIsLoaded()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ArrivedAtLoadingPlace,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.LoadTheVehicle(order.Id, driver.Id);

            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ArrivedAtLoadingPlace
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.Service.LoadTheVehicle(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void LoadTheVehicleWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var anotherDriver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ArrivedAtLoadingPlace,
                DriverId = driver.Id
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.LoadTheVehicle(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void LoadTheVehicleWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.LoadTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void DeliverTheVehicleResultVehicleIsDelivered()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsLoaded,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.DeliverTheVehicle(order.Id, driver.Id);

            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsLoaded
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.Service.DeliverTheVehicle(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void DeliverTheVehicleWhenDriverIsAnother()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var anotherDriver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsLoaded,
                DriverId = driver.Id
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.DeliverTheVehicle(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void DeliverTheVehicleWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ArrivedAtLoadingPlace,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.DeliverTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void ReceivePaymentResultPaymentIsReceived()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsDelivered,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.ReceivePayment(order.Id, driver.Id);

            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsDelivered
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.Service.ReceivePayment(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ReceivePaymentWhenAnotherDriver()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var anotherDriver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsDelivered,
                DriverId = driver.Id
            };

            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.Service.ReceivePayment(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void ReceivePaymentWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsLoaded,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.ReceivePayment(order.Id, driver.Id));
        }

        [Fact]
        public async void CompleteOrder()
        {
            var commonId = 1;
            var suite = new OrderStateServiceTestSuite();

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.PaymentIsReceived,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.Complete(order.Id, driver.Id);

            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await suite.Service.Cancel(order.Id, driver.Id);

            suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                DriverId = driver.Id
            };

            suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => suite.Service.Complete(order.Id, driver.Id));
        }
    }
}