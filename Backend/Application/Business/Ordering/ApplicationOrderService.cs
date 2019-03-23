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

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationOrderService : ApplicationTransactionService, IApplicationOrderService
    {
        public ApplicationOrderService(
            ITransactionService transactionService,
            IOrderService domainOrderService,
            IOrderStateService domainOrderStateService,
            IApplicationCargoService cargoService,
            IApplicationRouteService routeService,
            IApplicationCustomerService customerService,
            IApplicationBillService billService,
            IApplicationOrderValidatorService validatorService)
            : base(transactionService)
        {
            DomainOrderService = domainOrderService;
            DomainOrderStateService = domainOrderStateService;
            CargoService = cargoService;
            RouteService = routeService;
            CustomerService = customerService;
            BillService = billService;
            ValidatorService = validatorService;
        }

        protected IOrderService DomainOrderService { get; }

        protected IOrderStateService DomainOrderStateService { get; }


        protected IApplicationCargoService CargoService { get; }

        protected IApplicationRouteService RouteService { get; }

        protected IApplicationCustomerService CustomerService { get; }

        protected IApplicationBillService BillService { get; }

        protected IApplicationOrderValidatorService ValidatorService { get; }

        public async Task<Order> CreateOrder(BookingAM booking)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainCustomer = await CustomerService.GetDomainCustomer(booking.Customer);
                    var domainCargo = await CargoService.CreateDomainCargo(booking.Cargo);

                    var orderRoute = await RouteService.GetRoute(booking.RootAddress, booking.Waypoints);
                    var orderBill = await BillService.CalculateBill(booking.Bill.Info, booking.Bill.Basket);

                    await ValidatorService.Validate(booking, orderRoute, orderBill);

                    var domainRoute = await RouteService.CreateDomainRoute(orderRoute);
                    var domainBill = await BillService.CreateDomainBill(orderBill);

                    var result = await DomainOrderService.Create();
                    await DomainOrderStateService.New(
                        result.Id,
                        booking.TimeOfDelivery,
                        domainCustomer.Id,
                        domainCargo.Id,
                        domainRoute.Id,
                        domainBill.Id);

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

        public Task Accept(int orderId, int moderatorId)
        {
            return DomainOrderStateService.Accept(orderId, moderatorId);
        }

        public Task ReadyToTrade(int orderId, int moderatorId)
        {
            return DomainOrderStateService.ReadyToTrade(orderId, moderatorId);
        }

        public Task Trade(int orderId)
        {
            return DomainOrderStateService.Trade(orderId);
        }

        public Task AssignToDispatcher(int orderId, int dispatcherId)
        {
            return DomainOrderStateService.AssignToDispatcher(orderId, dispatcherId);
        }

        public Task AssignToDriver(int orderId, int dispatcherId, int driverId)
        {
            return DomainOrderStateService.AssignToDriver(orderId, dispatcherId, driverId);
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

        public async Task<ICollection<OrderGroupAM>> GetOrderGroupsByStatuses(OrderStatus[] statuses)
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
                    catch(Exception e)
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

        public async Task<ICollection<OrderInfoAM>> GetOrdersByStatus(OrderStatus status)
        {
            var result = new ConcurrentBag<OrderInfoAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            var domainOrdersStates = await GetDomainStatesByCurrentStatus(status);

            await domainOrdersStates.ParallelForEachAsync(
                async domainOrderState =>
                {
                    try
                    {
                        //var cargo = await CargoService.GetEntity(domainOrder.CargoId);
                        var routeTitle = await RouteService.GetShortTitle(domainOrderState.RouteId);
                        var totalDistance = await RouteService.GetTotalDistance(domainOrderState.RouteId);
                        var totalCost = await BillService.GetTotalCost(domainOrderState.BillId);

                        var orderInfo = new OrderInfoAM
                        {
                            TimeOfDelivery = domainOrderState.TimeOfDelivery,
                            Id = domainOrderState.OrderId,
                            Title = routeTitle,
                            Cost = totalCost,
                            Distance = totalDistance
                        };

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