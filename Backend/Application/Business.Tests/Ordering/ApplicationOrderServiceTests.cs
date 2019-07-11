using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Ordering;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Ordering;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests
{
    public class ApplicationOrderServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationOrderServiceTestSuite()
        {
            DomainOrderServiceMock = new Mock<IOrderService>();
            DomainOrderStateServiceMock = new Mock<IOrderStateService>();
            DomainMarketServiceMock = new Mock<IMarketService>();
            CargoServiceMock = new Mock<IApplicationCargoService>();
            RouteServiceMock = new Mock<IApplicationRouteService>();
            UserServiceMock = new Mock<IApplicationUserService>();
            BillServiceMock = new Mock<IApplicationBillService>();
            OrderValidatorServiceMock = new Mock<IApplicationOrderValidatorService>();
            AddressServiceMock = new Mock<IApplicationAddressService>();

            OrderServiceMock = new Mock<ApplicationOrderService>(
                TransactionServiceMock.Object,
                DomainOrderServiceMock.Object,
                DomainOrderStateServiceMock.Object,
                DomainMarketServiceMock.Object,
                CargoServiceMock.Object,
                RouteServiceMock.Object,
                UserServiceMock.Object,
                BillServiceMock.Object,
                OrderValidatorServiceMock.Object,
                AddressServiceMock.Object);
            OrderServiceMock.CallBase = true;
        }

        public Mock<ApplicationOrderService> OrderServiceMock { get; }

        public IApplicationOrderService OrderService => OrderServiceMock.Object;

        public Mock<IOrderService> DomainOrderServiceMock { get; }

        public Mock<IOrderStateService> DomainOrderStateServiceMock { get; }

        public Mock<IMarketService> DomainMarketServiceMock { get; }

        public Mock<IApplicationCargoService> CargoServiceMock { get; }

        public Mock<IApplicationRouteService> RouteServiceMock { get; }

        public Mock<IApplicationUserService> UserServiceMock { get; }

        public Mock<IApplicationBillService> BillServiceMock { get; }

        public Mock<IApplicationOrderValidatorService> OrderValidatorServiceMock { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }
    }

    public class ApplicationOrderServiceTests : BaseServiceTests<ApplicationOrderServiceTestSuite>
    {
        [Fact]
        public async Task GetOrderGroupsByStatusesResultStatusesCountEqualsOrderGroupCount()
        {
            var statusesArray = new[] { OrderStatus.New, OrderStatus.Accepted, OrderStatus.IsCarriedOut };

            var orderGroups = await Suite.OrderService.GetGroupsByStatuses(statusesArray);

            Assert.Equal(3, orderGroups.Count());
        }

        [Fact]
        public async Task GetOrderGroupsByStatusesResultEachGroupCorrespondsToRequestedStatus()
        {
            var statusesArray = new[] { OrderStatus.New, OrderStatus.IsCarriedOut, OrderStatus.Accepted };

            var orderGroups = await Suite.OrderService.GetGroupsByStatuses(statusesArray);

            Assert.Equal(statusesArray[0], orderGroups.ElementAt(0).Status);
            Assert.Equal(statusesArray[1], orderGroups.ElementAt(1).Status);
            Assert.Equal(statusesArray[2], orderGroups.ElementAt(2).Status);
        }

        [Fact]
        public async Task GetOrderGroupsByStatusesResultRightOrdersCountInGroup()
        {
            var sourceData = new[] {
                new { Status = OrderStatus.Accepted, Count = 1 },
                new { Status = OrderStatus.New, Count = 3 },
                new { Status = OrderStatus.ReadyForTrade, Count = 2 }
            };

            Suite.DomainOrderStateServiceMock
                .Setup(m => m.GetCountByCurrentStatus(It.IsAny<OrderStatus>()))
                .Returns<OrderStatus>(status => Task.FromResult(sourceData.First(item => item.Status.Equals(status)).Count));

            var orderGroups = await Suite.OrderService.GetGroupsByStatuses(sourceData.Select(s => s.Status).ToArray());

            Assert.Equal(sourceData[0].Count, orderGroups.ElementAt(0).Count);
            Assert.Equal(sourceData[1].Count, orderGroups.ElementAt(1).Count);
            Assert.Equal(sourceData[2].Count, orderGroups.ElementAt(2).Count);
        }

        [Fact]
        public async Task GetOrdersByNewStatusResultOrderListWithNewStatus()
        {
            var commonId = 1;

            var domainCargos = new List<Cargo> {
                new Cargo { Id = commonId++ },
                new Cargo { Id = commonId++ },
                new Cargo { Id = commonId++ }
            };

            var domainOrders = new List<Order> {
                new Order { Id = commonId++ },
                new Order { Id = commonId++ },
                new Order { Id = commonId++ }
            };

            var domainOrdersStates = new List<OrderState> {
                new OrderState { Id = commonId, OrderId = domainOrders[0].Id, CargoId = domainCargos[0].Id, RouteId = commonId++, TimeOfDelivery = new DateTime (2018, 6, 2, 11, 55, 3) },
                new OrderState { Id = commonId, OrderId = domainOrders[1].Id, CargoId = domainCargos[1].Id, RouteId = commonId++, TimeOfDelivery = new DateTime (2018, 6, 3, 12, 55, 3) },
                new OrderState { Id = commonId, OrderId = domainOrders[2].Id, CargoId = domainCargos[2].Id, RouteId = commonId++, TimeOfDelivery = new DateTime (2018, 6, 4, 13, 55, 3) }
            };

            var routesData = new[] {
                new { Id = domainOrdersStates[0].RouteId, ShortTitle = "Рыбинск - Москва" },
                new { Id = domainOrdersStates[1].RouteId, ShortTitle = "Москва" },
                new { Id = domainOrdersStates[2].RouteId, ShortTitle = "Ярославль - Вологда" }
            };

            Suite.DomainOrderStateServiceMock
                .Setup(m => m.GetByCurrentStatus(OrderStatus.New))
                .ReturnsAsync(domainOrdersStates);

            Suite.CargoServiceMock
                .Setup(m => m.GetDomainCargo(It.IsAny<int>()))
                .Returns<int>(cargoId => Task.FromResult(domainCargos.FirstOrDefault(c => c.Id.Equals(cargoId))));

            Suite.RouteServiceMock
                .Setup(m => m.GetShortTitle(It.IsAny<int>()))
                .Returns<int>(routeId => Task.FromResult(routesData.FirstOrDefault(r => r.Id.Equals(routeId)).ShortTitle));

            Suite.DomainOrderServiceMock
                .Setup(m => m.Get(It.IsAny<int[]>()))
                .ReturnsAsync(domainOrders);

            var orders = await Suite.OrderService.GetGroupByStatus(OrderStatus.New);

            for (var i = 0; i < domainOrders.Count; i++)
            {
                Assert.Equal(domainOrders[i].Id, orders.ElementAt(i).Id);
                Assert.Equal(domainOrdersStates[i].TimeOfDelivery, orders.ElementAt(i).TimeOfDelivery);
                Assert.Equal(routesData[i].ShortTitle, orders.ElementAt(i).Title);
            }
        }

        [Fact]
        public async Task GetDetailInfo()
        {
            var commonId = 1;

            var orderId = commonId++;
            var domainOrderState = new OrderState
            {
                Id = commonId++,
                OrderId = orderId,
                CargoId = commonId++,
                BillId = commonId++,
                RouteId = commonId
            };

            var cargo = new CargoAM();
            var route = new RouteAM();
            var bill = new BillAM();

            var orderInfo = new OrderInfoAM
            {
                Id = orderId,
            };

            Suite.DomainOrderStateServiceMock
                .Setup(m => m.GetCurrentState(orderId))
                .ReturnsAsync(domainOrderState);
            Suite.OrderServiceMock
                .Setup(m => m.GetInfo(domainOrderState))
                .ReturnsAsync(orderInfo);
            Suite.CargoServiceMock
                .Setup(m => m.GetCargo(domainOrderState.CargoId))
                .ReturnsAsync(cargo);
            Suite.RouteServiceMock
                .Setup(m => m.GetRoute(domainOrderState.RouteId))
                .ReturnsAsync(route);
            Suite.BillServiceMock
                .Setup(m => m.GetBill(domainOrderState.BillId))
                .ReturnsAsync(bill);

            var result = await Suite.OrderService.GetDetailInfo(orderId);

            Assert.Equal(cargo, result.Cargo);
            Assert.Equal(route, result.Route);
            Assert.Equal(bill, result.Bill);
        }

        [Fact]
        public async Task CreateOrderWithDispatcher()
        {
            var commonId = 1;

            var genDispatcher = new Dispatcher { Id = commonId++ };
            var domainMarket = new Market { Id = commonId++, AddressId = commonId++ };

            var booking = new BookingAM
            {
                TimeOfDelivery = DateTime.MinValue,
                MarketId = domainMarket.Id,
                Bill = new BillAM
                {
                    Info = new BillInfoAM(),
                    Basket = new BasketAM()
                },
                Customer = new CustomerAM(),
                Cargo = new CargoAM(),
                Waypoints = new WaypointsAM()
            };

            var route = new RouteAM();
            var bill = new BillAM();
            var marketAddress = new AddressAM();
            var domainRoute = new Route { Id = commonId++ };
            var domainCustomer = new Customer { Id = commonId++ };
            var domainCargo = new Cargo { Id = commonId++ };
            var domainBill = new Bill { Id = commonId++ };
            var domainOrder = new Order { Id = commonId++ };

            Suite.UserServiceMock
                .Setup(m => m.GetOrCreateDomainCustomer(booking.Customer))
                .ReturnsAsync(domainCustomer);
            Suite.CargoServiceMock
                .Setup(m => m.CreateDomainCargo(booking.Cargo))
                .ReturnsAsync(domainCargo);
            Suite.RouteServiceMock
                .Setup(m => m.FindRoute(marketAddress, booking.Waypoints))
                .ReturnsAsync(route);
            Suite.RouteServiceMock
                .Setup(m => m.CreateDomainRoute(route))
                .ReturnsAsync(domainRoute);
            Suite.BillServiceMock
                .Setup(m => m.CalculateBill(booking.Bill.Info, booking.Bill.Basket))
                .ReturnsAsync(bill);
            Suite.BillServiceMock
                .Setup(m => m.CreateDomainBill(bill))
                .ReturnsAsync(domainBill);
            Suite.DomainOrderServiceMock
                .Setup(m => m.Create())
                .ReturnsAsync(domainOrder);
            Suite.DomainMarketServiceMock
                .Setup(m => m.Get(booking.MarketId))
                .ReturnsAsync(domainMarket);
            Suite.AddressServiceMock
                .Setup(m => m.GetAddress(domainMarket.AddressId))
                .ReturnsAsync(marketAddress);
            Suite.UserServiceMock
                .Setup(m => m.GetDomainDispatcher(genDispatcher.Id))
                .ReturnsAsync(genDispatcher);

            var result = await Suite.OrderService.CreateOrder(booking, genDispatcher.Id);

            Suite.DomainOrderStateServiceMock
                .Verify(m => m.New(
                   domainOrder.Id,
                   domainMarket.Id,
                   booking.TimeOfDelivery,
                   domainCustomer.Id,
                   domainCargo.Id,
                   domainRoute.Id,
                   domainBill.Id));

            Suite.OrderValidatorServiceMock
                .Verify(m => m.Validate(booking, route, bill));

            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Accept(domainOrder.Id, genDispatcher.Id));

            Assert.Equal(domainOrder, result);
        }

        [Fact]
        public async void Accept()
        {
            var commonId = 1;
            var orderId = commonId++;
            var genDispatcher = commonId++;

            await Suite.OrderService.Accept(orderId, genDispatcher);

            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Accept(orderId, genDispatcher));
        }

        [Fact]
        public async void ReadyToTrade()
        {
            var commonId = 1;
            var orderId = commonId++;
            var genDispatcherId = commonId++;

            await Suite.OrderService.ReadyToTrade(orderId, genDispatcherId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ReadyToTrade(orderId, genDispatcherId));
        }

        [Fact]
        public async void Trade()
        {
            var commonId = 1;
            var orderId = commonId++;

            await Suite.OrderService.Trade(orderId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Trade(orderId));

        }

        [Fact]
        public async void AssignToDispatcher()
        {
            var commonId = 1;
            var orderId = commonId++;
            var dispatcherId = commonId++;

            await Suite.OrderService.AssignToSubDispatcher(orderId, dispatcherId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.AssignToSubDispatcher(orderId, dispatcherId));
        }

        [Fact]
        public async void AssignToDriver()
        {
            var commonId = 1;
            var orderId = commonId++;
            var subDispatcherId = commonId++;
            var driverId = commonId++;
            var vehicleId = commonId++;

            await Suite.OrderService.AssignToDriver(orderId, subDispatcherId, driverId, vehicleId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.AssignToDriver(orderId, subDispatcherId, driverId, vehicleId));
        }

        [Fact]
        public async void ConfirmByDriver()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.ConfirmByDriver(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ConfirmByDriver(orderId, driverId));
        }

        [Fact]
        public async void GoToCustomer()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.GoToCustomer(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.GoToCustomer(orderId, driverId));
        }

        [Fact]
        public async void ArriveAtLoadingPlace()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.ArriveAtLoadingPlace(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ArriveAtLoadingPlace(orderId, driverId));
        }

        [Fact]
        public async void LoadTheVehicle()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.LoadTheVehicle(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.LoadTheVehicle(orderId, driverId));
        }

        [Fact]
        public async void DeliverTheVehicle()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.DeliverTheVehicle(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.DeliverTheVehicle(orderId, driverId));
        }

        [Fact]
        public async void ReceivePayment()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.ReceivePayment(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ReceivePayment(orderId, driverId));
        }

        [Fact]
        public async void Complete()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.Complete(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Complete(orderId, driverId));
        }

        [Fact]
        public async void Cancel()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderService.Cancel(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Cancel(orderId, driverId));
        }
    }
}