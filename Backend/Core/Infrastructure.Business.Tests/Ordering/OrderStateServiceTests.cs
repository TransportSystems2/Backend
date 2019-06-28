using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Organization;
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
            VehicleServiceMock = new Mock<IVehicleService>();

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
                MarketServiceMock.Object,
                VehicleServiceMock.Object);
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

        public Mock<IVehicleService> VehicleServiceMock { get; }
    }

    public class OrderStateServiceTests
    {
        public OrderStateServiceTests()
        {
            Suite = new OrderStateServiceTestSuite();
        }

        protected OrderStateServiceTestSuite Suite { get; }

        [Fact]
        public async void GetCurrentState()
        {
            var orderId = 1;

            Suite.OrderServiceMock
                .Setup(m => m.IsExist(orderId))
                .ReturnsAsync(true);

            var currentState = await Suite.Service.GetCurrentState(orderId);

            Suite.RepositoryMock
                .Verify(m => m.GetCurrentState(orderId));
        }

        [Fact]
        public async void GetCurrentStateForNotExistingOrder()
        {
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Order", () => Suite.Service.GetCurrentState(1));
        }

        [Fact]
        public async void GetStatesWithNewCurentStatus()
        {
            var status = OrderStatus.New;

            var newStates = await Suite.Service.GetByCurrentStatus(status);
            Suite.RepositoryMock
                .Verify(m => m.GetStatesByCurrentStatus(status));
        }

        [Fact]
        public async void GetAcceptedStatesCount()
        {
            var status = OrderStatus.Accepted;

            var count = await Suite.Service.GetCountByCurrentStatus(status);

            Suite.RepositoryMock
                .Verify(m => m.GetCountStatesByCurrentStatus(status));
        }

        [Fact]
        public async void New()
        {
            var commonId = 1;
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

            Suite.OrderServiceMock
                .Setup(m => m.IsExist(orderId))
                .ReturnsAsync(true);
            Suite.CustomerServiceMock
                .Setup(m => m.IsExist(customerId))
                .ReturnsAsync(true);
            Suite.CargoServiceMock
                .Setup(m => m.IsExist(cargoId))
                .ReturnsAsync(true);
            Suite.RouteServiceMock
                .Setup(m => m.IsExist(routeId))
                .ReturnsAsync(true);
            Suite.BillServiceMock
                .Setup(m => m.IsExist(billId))
                .ReturnsAsync(true);
            Suite.MarketServiceMock
                .Setup(m => m.Get(market.Id))
                .ReturnsAsync(market);

            await Suite.Service.New(orderId, market.Id, timeOfDelivery, customerId, cargoId, routeId, billId);
            Suite.RepositoryMock
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
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.New,
                GenCompanyId = companyId
            };
            var genDispatcher = new Dispatcher { Id = commonId++, CompanyId = companyId };

            Suite.DispatcherServiceMock
                .Setup(m => m.Get(genDispatcher.Id))
                .ReturnsAsync(genDispatcher);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.Accept(order.Id, genDispatcher.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.Accepted)
                    && s.GenDispatcherId.Equals(genDispatcher.Id))));
        }

        [Fact]
        public async void AcceptWhenOrderDoesNotExist()
        {
            var commonId = 1;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };
            var moderator = new Moderator { Id = commonId++ };

            Suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(false);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Order",
                () => Suite.Service.Accept(order.Id, moderator.Id));
        }

        [Fact]
        public async void AcceptWhenGenDispatcerDoesNotExist()
        {
            var commonId = 1;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };
            var genDispatcher = new Dispatcher { Id = commonId++ };

            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(genDispatcher.Id))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "GenDispatcher",
                () => Suite.Service.Accept(order.Id, genDispatcher.Id));
        }

        [Fact]
        public async void AcceptWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.AssignedDispatcher };
            var moderator = new Moderator { Id = commonId++ };

            Suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderator.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(
                () => Suite.Service.Accept(order.Id, moderator.Id));
        }

        [Fact]
        public async void ReadyToTrade()
        {
            var commonId = 1;

            var genDispatcher = new Dispatcher { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.Accepted,
                GenDispatcherId = genDispatcher.Id
            };

            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(genDispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.ReadyToTrade(order.Id, genDispatcher.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.ReadyForTrade))));
        }

        [Fact]
        public async void ReadyToTradeWhenDispatcherIsOther()
        {
            var commonId = 1;

            var genDispatcher = new Dispatcher { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.Accepted,
                GenDispatcherId = -1
            };

            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(genDispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<AccessViolationException>(() => Suite.Service.ReadyToTrade(order.Id, genDispatcher.Id));
        }

        [Fact]
        public async void Trade()
        {
            var commonId = 1;

            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.ReadyForTrade };

            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.Trade(order.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.SentToTrading))));
        }

        [Fact]
        public async void TradeWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.Accepted };

            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.Trade(order.Id));
        }

        [Fact]
        public async void AssignOrderToSubDispatcher()
        {
            var commonId = 1;

            var subCompanyId = commonId++;
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.Accepted };

            Suite.DispatcherServiceMock
                .Setup(m => m.Get(subDispatcher.Id))
                .ReturnsAsync(subDispatcher);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.AssignToSubDispatcher(order.Id, subDispatcher.Id);

            Suite.RepositoryMock
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

            var dispatcher = new Dispatcher { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState { Id = commonId++, OrderId = order.Id, Status = OrderStatus.New };

            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.AssignToSubDispatcher(order.Id, dispatcher.Id));
        }

        [Fact]
        public async void AssignToDriver()
        {
            var commonId = 1;

            var subCompanyId = commonId++;

            var order = new Order { Id = commonId++ };
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var driver = new Driver { Id = commonId++, CompanyId = subCompanyId };
            var vehicle = new Vehicle { Id = commonId++, CompanyId = subCompanyId };
            var currentState = new OrderState
            {
                Id = commonId++,
                SubCompanyId = subCompanyId,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);
            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);
            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.AssignToDriver(order.Id, subDispatcher.Id, driver.Id, vehicle.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(s => 
                    s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.AssignedDriver)
                    && s.DriverId.Equals(driver.Id)
                    && s.VehicleId.Equals(vehicle.Id))));
        }

        [Fact]
        public async void AssignToDriverWnenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var subCompanyId = commonId++;

            var order = new Order { Id = commonId++ };
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var driver = new Driver { Id = commonId++, CompanyId = subCompanyId };
            var vehicle = new Vehicle { Id = commonId++, CompanyId = subCompanyId };
            var currentState = new OrderState
            {
                Id = commonId++,
                SubCompanyId = subCompanyId,
                OrderId = order.Id,
                Status = OrderStatus.New,
                SubDispatcherId = subDispatcher.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);
            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);
            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => 
                Suite.Service.AssignToDriver(
                    order.Id,
                    subDispatcher.Id,
                    driver.Id,
                    vehicle.Id));
        }

        [Fact]
        public async void AssignToDriverWhenAnotherDispatcher()
        {
            var commonId = 1;

            var subCompanyId = commonId++;

            var anotherDispatcherId = commonId++;
            var order = new Order { Id = commonId++ };
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var driver = new Driver { Id = commonId++, CompanyId = subCompanyId };
            var vehicle = new Vehicle { Id = commonId++, CompanyId = subCompanyId };
            var currentState = new OrderState
            {
                Id = commonId++,
                SubCompanyId = subCompanyId,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);
            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);
            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<AccessViolationException>(() => 
                Suite.Service.AssignToDriver(
                    order.Id,
                    anotherDispatcherId,
                    driver.Id,
                    vehicle.Id));
        }

        [Fact]
        public async void AssignToDriverWhenDriverDoesNotExist()
        {
            var commonId = 1;

            var subCompanyId = commonId++;

            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var vehicle = new Vehicle { Id = commonId++, CompanyId = subCompanyId };
            var currentState = new OrderState
            {
                Id = commonId++,
                SubCompanyId = subCompanyId,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.Get(notExistingDriverId))
                .Returns(Task.FromResult<Driver>(null));
            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);
            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Driver",
                () => Suite.Service.AssignToDriver(order.Id, subDispatcher.Id, notExistingDriverId, vehicle.Id));
        }

        [Fact]
        public async void AssignToDriverWhenDispatcherDoesNotFound()
        {
            var commonId = 1;

            var subCompanyId = commonId++;

            var notExistingDispatcher = commonId++;
            var order = new Order { Id = commonId++ };
            var driver = new Driver { Id = commonId++, CompanyId = subCompanyId };
            var vehicle = new Vehicle { Id = commonId++, CompanyId = subCompanyId };
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var currentState = new OrderState
            {
                Id = commonId++,
                SubCompanyId = subCompanyId,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);
            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);
            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(notExistingDispatcher))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "SubDispatcher",
                () => Suite.Service.AssignToDriver(
                    order.Id,
                    notExistingDispatcher,
                    driver.Id,
                    vehicle.Id));
        }

        [Fact]
        public async void AssignToDriverWhenVehicleDoesNotFound()
        {
            var commonId = 1;

            var subCompanyId = commonId++;

            var order = new Order { Id = commonId++ };
            var driver = new Driver { Id = commonId++, CompanyId = subCompanyId };
            var notExistingVehicleId = commonId++;
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var currentState = new OrderState
            {
                Id = commonId++,
                SubCompanyId = subCompanyId,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);
            Suite.VehicleServiceMock
                .Setup(m => m.Get(notExistingVehicleId))
                .Returns(Task.FromResult<Vehicle>(null));
            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                "Vehicle",
                () => Suite.Service.AssignToDriver(
                    order.Id,
                    subDispatcher.Id,
                    driver.Id,
                    notExistingVehicleId));
        }

        [Fact]
        public async void AssignToDriverWhenVehicleIsOwnedAnotherCompany()
        {
            var commonId = 1;

            var subCompanyId = commonId++;
            var anotherCompanyId = commonId++;

            var order = new Order { Id = commonId++ };
            var driver = new Driver { Id = commonId++, CompanyId = subCompanyId };
            var subDispatcher = new Dispatcher { Id = commonId++, CompanyId = subCompanyId };
            var vehicle = new Vehicle { Id = commonId++, CompanyId = anotherCompanyId };
            var currentState = new OrderState
            {
                Id = commonId++,
                SubCompanyId = subCompanyId,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                SubDispatcherId = subDispatcher.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.Get(driver.Id))
                .ReturnsAsync(driver);
            Suite.VehicleServiceMock
                .Setup(m => m.Get(vehicle.Id))
                .ReturnsAsync(vehicle);
            Suite.DispatcherServiceMock
                .Setup(m => m.IsExist(subDispatcher.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<AccessViolationException>(
                () => Suite.Service.AssignToDriver(
                    order.Id,
                    subDispatcher.Id,
                    driver.Id,
                    vehicle.Id));
        }

        [Fact]
        public async void ConfirmByDriver()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDriver,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.ConfirmByDriver(order.Id, driver.Id);
            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.ConfirmedByDriver))));
        }

        [Fact]
        public async void ConfirmWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDriver
            };

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => Suite.Service.ConfirmByDriver(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ConfirmWhenDriverIsAnother()
        {
            var commonId = 1;

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

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => Suite.Service.ConfirmByDriver(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void ConfirmWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.ConfirmByDriver(order.Id, driver.Id));
        }

        [Fact]
        public async void GoToCustomerResultWentToCustomer()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ConfirmedByDriver,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.GoToCustomer(order.Id, driver.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.WentToCustomer))));
        }

        [Fact]
        public async void GoToCustomerWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ConfirmedByDriver
            };

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => Suite.Service.GoToCustomer(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void GoToCustomerWhenAnotherDriver()
        {
            var commonId = 1;

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

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => Suite.Service.GoToCustomer(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void GoToCustomerWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDriver,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.GoToCustomer(order.Id, driver.Id));
        }

        [Fact]
        public async void ArriveAtLoading()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.ArriveAtLoadingPlace(order.Id, driver.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.ArrivedAtLoadingPlace))));
        }

        [Fact]
        public async void ArriveAtLoadingWhenDriverDoesNotExist()
        {
            var commonId = 1;

            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer
            };

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => Suite.Service.ArriveAtLoadingPlace(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ArriveAtLoadingWhenDriverIsAnother()
        {
            var commonId = 1;

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

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => Suite.Service.ArriveAtLoadingPlace(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void ArriveAtLoadingPlaceWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ConfirmedByDriver,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.ArriveAtLoadingPlace(order.Id, driver.Id));
        }

        [Fact]
        public async void LoadTheVehicleResultVehicleIsLoaded()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ArrivedAtLoadingPlace,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.LoadTheVehicle(order.Id, driver.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.VehicleIsLoaded))));
        }

        [Fact]
        public async void LoadTheVehicleWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ArrivedAtLoadingPlace
            };

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => Suite.Service.LoadTheVehicle(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void LoadTheVehicleWhenAnotherDriver()
        {
            var commonId = 1;

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

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => Suite.Service.LoadTheVehicle(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void LoadTheVehicleWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.LoadTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void DeliverTheVehicleResultVehicleIsDelivered()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsLoaded,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.DeliverTheVehicle(order.Id, driver.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.VehicleIsDelivered))));
        }

        [Fact]
        public async void DeliverTheVehicleWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsLoaded
            };

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => Suite.Service.DeliverTheVehicle(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void DeliverTheVehicleWhenDriverIsAnother()
        {
            var commonId = 1;

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

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => Suite.Service.DeliverTheVehicle(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void DeliverTheVehicleWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.ArrivedAtLoadingPlace,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.DeliverTheVehicle(order.Id, driver.Id));
        }

        [Fact]
        public async void ReceivePaymentResultPaymentIsReceived()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsDelivered,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.ReceivePayment(order.Id, driver.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.PaymentIsReceived))));
        }

        [Fact]
        public async void ReceivePaymentWhenDriverDoesNotExist()
        {
            var commonId = 1;
            var notExistingDriverId = commonId++;
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsDelivered
            };

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(notExistingDriverId))
                .ReturnsAsync(false);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => Suite.Service.ReceivePayment(order.Id, notExistingDriverId));
        }

        [Fact]
        public async void ReceivePaymentWhenAnotherDriver()
        {
            var commonId = 1;

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

            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);
            Suite.DriverServiceMock
                .Setup(m => m.IsExist(anotherDriver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<AccessViolationException>(() => Suite.Service.ReceivePayment(order.Id, anotherDriver.Id));
        }

        [Fact]
        public async void ReceivePaymentWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.VehicleIsLoaded,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.ReceivePayment(order.Id, driver.Id));
        }

        [Fact]
        public async void CompleteOrder()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.PaymentIsReceived,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.Complete(order.Id, driver.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.Completed))));
        }

        [Fact]
        public async void Cancel()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.WentToCustomer,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Suite.Service.Cancel(order.Id, driver.Id);

            Suite.RepositoryMock
                .Verify(m => m.Add(It.Is<OrderState>(
                    s => s.OrderId.Equals(order.Id)
                    && s.Status.Equals(OrderStatus.AssignedDispatcher))));
        }

        [Fact]
        public async void CompleteWhenOrderStatusIsUnsuitable()
        {
            var commonId = 1;

            var driver = new Driver { Id = commonId++ };
            var order = new Order { Id = commonId++ };
            var currentState = new OrderState
            {
                Id = commonId++,
                OrderId = order.Id,
                Status = OrderStatus.AssignedDispatcher,
                DriverId = driver.Id
            };

            Suite.DriverServiceMock
                .Setup(m => m.IsExist(driver.Id))
                .ReturnsAsync(true);
            Suite.OrderServiceMock
                .Setup(m => m.IsExist(order.Id))
                .ReturnsAsync(true);
            Suite.RepositoryMock
                .Setup(m => m.GetCurrentState(order.Id))
                .ReturnsAsync(currentState);

            await Assert.ThrowsAsync<OrderStatusException>(() => Suite.Service.Complete(order.Id, driver.Id));
        }
    }
}