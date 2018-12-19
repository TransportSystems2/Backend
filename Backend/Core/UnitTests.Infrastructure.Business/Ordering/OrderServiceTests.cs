using Moq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
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

namespace TransportSystems.UnitTests.Infrastructure.Business
{
    public class OrderServiceTestSuite
    {
        public OrderServiceTestSuite()
        {
            OrderRepositoryMock = new Mock<IOrderRepository>();
            ModeratorServiceMock = new Mock<IModeratorService>();
            DispatcherServiceMock = new Mock<IDispatcherService>();
            DriverServiceMock = new Mock<IDriverService>();
            CustomerServiceMock = new Mock<ICustomerService>();
            CargoServiceMock = new Mock<ICargoService>();
            RouteServiceMock = new Mock<IRouteService>();
            BillServiceMock = new Mock<IBasketService>();

            OrderService = new OrderService(
                OrderRepositoryMock.Object,
                ModeratorServiceMock.Object,
                DispatcherServiceMock.Object,
                DriverServiceMock.Object,
                CustomerServiceMock.Object,
                CargoServiceMock.Object,
                RouteServiceMock.Object,
                BillServiceMock.Object);
        }

        public IOrderService OrderService { get; }

        public Mock<IModeratorService> ModeratorServiceMock { get; }

        public Mock<IDispatcherService> DispatcherServiceMock { get; }

        public Mock<IDriverService> DriverServiceMock { get; }

        public Mock<ICustomerService> CustomerServiceMock { get; }

        public Mock<ICargoService> CargoServiceMock { get; }

        public Mock<IRouteService> RouteServiceMock { get; }

        public Mock<IOrderRepository> OrderRepositoryMock { get; }

        public Mock<IBasketService> BillServiceMock { get; }
    }

    public class OrderServiceTest
    {
        [Fact]
        public async void CreateOrder()
        {
            var suite = new OrderServiceTestSuite();

            var timeOfDelivery = DateTime.MinValue;
            var customerId = 2;
            var cargoId = 3;
            var routeId = 4;
            var billId = 5;

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

            var newOrder = await suite.OrderService.Create(timeOfDelivery, customerId, cargoId, routeId, billId);

            suite.OrderRepositoryMock
                .Verify(m => m.Add(It.Is<Order>(
                    o => o.TimeOfDelivery.Equals(timeOfDelivery)
                    && o.CustomerId.Equals(customerId)
                    && o.CargoId.Equals(cargoId)
                    && o.RouteId.Equals(routeId)
                    && o.BillId.Equals(billId))));
        }

        [Fact]
        public async void AssignModerator()
        {
            var suite = new OrderServiceTestSuite();

            var commonId = 1;
            var order = new Order { Id = commonId++ };
            var moderatorId = commonId;

            suite.OrderRepositoryMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderatorId))
                .ReturnsAsync(true);

            await suite.OrderService.AssignModerator(order.Id, moderatorId);

            Assert.Equal(moderatorId, order.ModeratorId);
        }

        [Fact]
        public async void AssignModeratorWhenOrderDoesNotExist()
        {
            var suite = new OrderServiceTestSuite();
            var moderatorId = 1;

            suite.OrderRepositoryMock
                .Setup(m => m.Get(It.IsAny<int>()))
                .Returns(Task.FromResult<Order>(null));
            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(moderatorId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Order", () => suite.OrderService.AssignModerator(2, moderatorId));
        }

        [Fact]
        public async void AssignModeratorWhenModeratorDoesNotExist()
        {
            var suite = new OrderServiceTestSuite();
            var order = new Order { Id = 1 };

            suite.OrderRepositoryMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.ModeratorServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Moderator", () => suite.OrderService.AssignModerator(order.Id, 2));
        }

        [Fact]
        public async void AssignDispatcher()
        {
            var suite = new OrderServiceTestSuite();

            var commonId = 1;
            var order = new Order { Id = commonId++ };
            var dispatcherId = commonId;

            suite.OrderRepositoryMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcherId))
                .ReturnsAsync(true);

            await suite.OrderService.AssignDispatcher(order.Id, dispatcherId);

            Assert.Equal(dispatcherId, order.DispatcherId);
        }

        [Fact]
        public async void AssignDispatcherWhenOrderDoesNotExist()
        {
            var suite = new OrderServiceTestSuite();
            var dispatcherId = 1;

            suite.OrderRepositoryMock
                .Setup(m => m.Get(It.IsAny<int>()))
                .Returns(Task.FromResult<Order>(null));
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcherId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Order", () => suite.OrderService.AssignDispatcher(2, dispatcherId));
        }

        [Fact]
        public async void AssignDispatcherWhenDispatcherDoesNotExist()
        {
            var suite = new OrderServiceTestSuite();
            var order = new Order { Id = 1 };

            suite.OrderRepositoryMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Dispatcher", () => suite.OrderService.AssignDispatcher(order.Id, 2));
        }

        [Fact]
        public async void AssignDriver()
        {
            var suite = new OrderServiceTestSuite();

            var commonId = 1;
            var order = new Order { Id = commonId++ };
            var driverId = commonId++;

            suite.OrderRepositoryMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(driverId))
                .ReturnsAsync(true);

            await suite.OrderService.AssignDriver(order.Id, driverId);

            Assert.Equal(driverId, order.DriverId);
        }

        [Fact]
        public async void AssignDriverrWhenOrderDoesNotExist()
        {
            var suite = new OrderServiceTestSuite();
            var driverId = 1;

            suite.OrderRepositoryMock
                .Setup(m => m.Get(It.IsAny<int>()))
                .Returns(Task.FromResult<Order>(null));
            suite.DriverServiceMock
                .Setup(m => m.IsExist(driverId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Order", () => suite.OrderService.AssignDriver(2, driverId));
        }

        [Fact]
        public async void AssignDriverWhenDriverDoesNotExist()
        {
            var suite = new OrderServiceTestSuite();
            var order = new Order { Id = 1 };

            suite.OrderRepositoryMock
                .Setup(m => m.Get(order.Id))
                .ReturnsAsync(order);
            suite.DriverServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Driver", () => suite.OrderService.AssignDriver(order.Id, 2));
        }
    }
}