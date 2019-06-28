using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Ordering;
using TransportSystems.Backend.Application.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationOrderService : ApplicationTransactionService, IApplicationOrderService
    {
        public ApplicationOrderService(
            ITransactionService transactionService,
            IOrderService domainOrderService,
            IOrderStateService domainOrderStateService,
            IMarketService domainMarketService,
            IApplicationCargoService cargoService,
            IApplicationRouteService routeService,
            IApplicationUserService userService,
            IApplicationBillService billService,
            IApplicationOrderValidatorService validatorService,
            IApplicationAddressService addressService)
            : base(transactionService)
        {
            DomainOrderService = domainOrderService;
            DomainOrderStateService = domainOrderStateService;
            DomainMarketService = domainMarketService;
            CargoService = cargoService;
            RouteService = routeService;
            UserService = userService;
            BillService = billService;
            ValidatorService = validatorService;
            AddressService = addressService;
        }

        protected IOrderService DomainOrderService { get; }

        protected IOrderStateService DomainOrderStateService { get; }

        protected IMarketService DomainMarketService { get; }

        protected IApplicationCargoService CargoService { get; }

        protected IApplicationRouteService RouteService { get; }

        protected IApplicationUserService UserService { get; }

        protected IApplicationBillService BillService { get; }

        protected IApplicationOrderValidatorService ValidatorService { get; }

        protected IApplicationAddressService AddressService { get; }

        public async Task<Order> CreateOrder(BookingAM booking, int genDispatcherId)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainCustomer = await UserService.GetOrCreateDomainCustomer(booking.Customer);
                    var domainCargo = await CargoService.CreateDomainCargo(booking.Cargo);

                    var domainMarket = await DomainMarketService.Get(booking.MarketId);
                    var marketAddress = await AddressService.GetAddress(domainMarket.AddressId);

                    var orderRoute = await RouteService.FindRoute(marketAddress, booking.Waypoints);
                    var orderBill = await BillService.CalculateBill(booking.Bill.Info, booking.Bill.Basket);

                    await ValidatorService.Validate(booking, orderRoute, orderBill);

                    var domainRoute = await RouteService.CreateDomainRoute(orderRoute);
                    var domainBill = await BillService.CreateDomainBill(orderBill);

                    var result = await DomainOrderService.Create();
                    await DomainOrderStateService.New(
                        result.Id,
                        domainMarket.Id,
                        booking.TimeOfDelivery,
                        domainCustomer.Id,
                        domainCargo.Id,
                        domainRoute.Id,
                        domainBill.Id);

                    var domainGenDispatcher = await UserService.GetDomainDispatcher(genDispatcherId);
                    if (domainGenDispatcher != null)
                    {
                        await Accept(result.Id, genDispatcherId);
                    }

                    transaction.Commit();

                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task Accept(int orderId, int genDispatcherId)
        {
            await DomainOrderStateService.Accept(orderId, genDispatcherId);
        }

        public async Task ReadyToTrade(int orderId, int genDispatcherId)
        {
            await DomainOrderStateService.ReadyToTrade(orderId, genDispatcherId);
        }

        public Task Trade(int orderId)
        {
            return DomainOrderStateService.Trade(orderId);
        }

        public Task AssignToSubDispatcher(int orderId, int subDispatcherId)
        {
            return DomainOrderStateService.AssignToSubDispatcher(orderId, subDispatcherId);
        }

        public Task AssignToDriver(int orderId, int subDispatcherId, int driverId, int vehicleId)
        {
            return DomainOrderStateService.AssignToDriver(orderId, subDispatcherId, driverId, vehicleId);
        }

        public Task ConfirmByDriver(int orderId, int driverId)
        {
            return DomainOrderStateService.ConfirmByDriver(orderId, driverId);
        }

        public Task GoToCustomer(int orderId, int driverId)
        {
            return DomainOrderStateService.GoToCustomer(orderId, driverId);
        }

        public Task ArriveAtLoadingPlace(int orderId, int driverId)
        {
            return DomainOrderStateService.ArriveAtLoadingPlace(orderId, driverId);
        }

        public Task LoadTheVehicle(int orderId, int driverId)
        {
            return DomainOrderStateService.LoadTheVehicle(orderId, driverId);
        }

        public Task DeliverTheVehicle(int orderId, int driverId)
        {
            return DomainOrderStateService.DeliverTheVehicle(orderId, driverId);
        }

        public Task ReceivePayment(int orderId, int driverId)
        {
            return DomainOrderStateService.ReceivePayment(orderId, driverId);
        }

        public Task Complete(int orderId, int driverId)
        {
            return DomainOrderStateService.Complete(orderId, driverId);
        }

        public Task Cancel(int orderId, int driverId)
        {
            return DomainOrderStateService.Cancel(orderId, driverId);
        }

        public async Task<ICollection<OrderGroupAM>> GetGroupsByStatuses(OrderStatus[] statuses)
        {
            var result = new ConcurrentBag<OrderGroupAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            await statuses.ParallelForEachAsync(
                async status =>
                {
                    try
                    {
                        var stateOrderCount = await GetCountByCurrentStatus(status);
                        var group = new OrderGroupAM
                        {
                            Title = status.ToString(),
                            Status = status,
                            Count = stateOrderCount
                        };

                        result.Add(group);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                }
            );

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            var orderedResult = result.OrderBy(o => Array.IndexOf(statuses, o.Status)).ToList();

            return orderedResult;
        }

        public async Task<ICollection<Order>> GetDomainOrdersByStatus(OrderStatus status)
        {
            var ordersStates = await GetDomainStatesByCurrentStatus(status);
            var ordersIdArray = ordersStates.Select(s => s.OrderId).ToArray();

            return await DomainOrderService.Get(ordersIdArray);
        }

        public virtual async Task<OrderInfoAM> GetInfo(int orderId)
        {
            var domainCurrentState = await DomainOrderStateService.GetCurrentState(orderId);
            
            return await GetInfo(domainCurrentState);
        }

        public virtual async Task<OrderInfoAM> GetInfo(OrderState orderState)
        {
            //var cargo = await CargoService.GetEntity(domainOrder.CargoId);
            var routeTitle = await RouteService.GetShortTitle(orderState.RouteId);
            var totalDistance = await RouteService.GetTotalDistance(orderState.RouteId);
            var totalCost = await BillService.GetTotalCost(orderState.BillId);

            var result = new OrderInfoAM
            {
                TimeOfDelivery = orderState.TimeOfDelivery,
                Id = orderState.OrderId,
                Title = routeTitle,
                Cost = totalCost,
                Distance = totalDistance
            };

            return result;
        }

        public async Task<ICollection<OrderInfoAM>> GetGroupByStatus(OrderStatus status)
        {
            var result = new ConcurrentBag<OrderInfoAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            var domainOrdersStates = await GetDomainStatesByCurrentStatus(status);

            await domainOrdersStates.ParallelForEachAsync(
                async domainOrderState =>
                {
                    try
                    {
                        var orderInfo = await GetInfo(domainOrderState);

                        result.Add(orderInfo);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                }
            );

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            var orderedResult = result.OrderBy(o => o.Id).ToList();

            return orderedResult;
        }

        public async Task<DetailOrderInfoAM> GetDetailInfo(int orderId)
        {
            var domainOrderState = await DomainOrderStateService.GetCurrentState(orderId);
            if (domainOrderState == null)
            {
                throw new ArgumentException($"OrderId:{orderId} is null", "Order");
            }

            var orderInfo = await GetInfo(domainOrderState);
            var result = new DetailOrderInfoAM(orderInfo)
            {
                Cargo = await CargoService.GetCargo(domainOrderState.CargoId),
                Route = await RouteService.GetRoute(domainOrderState.RouteId),
                Bill = await BillService.GetBill(domainOrderState.BillId),
                Customer = await UserService.GetCustomer(domainOrderState.CustomerId)
            };

            return result;
        }

        private Task<ICollection<OrderState>> GetDomainStatesByCurrentStatus(OrderStatus status)
        {
            return DomainOrderStateService.GetByCurrentStatus(status);
        }

        private Task<int> GetCountByCurrentStatus(OrderStatus status)
        {
            return DomainOrderStateService.GetCountByCurrentStatus(status);
        }
    }
}