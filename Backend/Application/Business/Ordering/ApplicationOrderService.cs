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

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationOrderService : ApplicationTransactionService, IApplicationOrderService
    {
        public ApplicationOrderService(
            ITransactionService transactionService,
            IOrderService domainOrderService,
            IApplicationOrderStateService orderStateService,
            IApplicationCargoService cargoService,
            IApplicationRouteService routeService,
            IApplicationCustomerService customerService,
            IApplicationBillService billService)
            : base(transactionService)
        {
            DomainOrderService = domainOrderService;
            OrderStateService = orderStateService;
            CargoService = cargoService;
            RouteService = routeService;
            CustomerService = customerService;
            BillService = billService;
        }

        protected IOrderService DomainOrderService { get; }

        protected IApplicationOrderStateService OrderStateService { get; }

        protected IApplicationCargoService CargoService { get; }

        protected IApplicationRouteService RouteService { get; }

        protected IApplicationCustomerService CustomerService { get; }

        protected IApplicationBillService BillService { get; }

        public async Task<Order> CreateOrder(BookingAM booking)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainCustomer = await CustomerService.GetDomainCustomer(booking.Customer);
                    var domainCargo = await CargoService.CreateDomainCargo(booking.Cargo);
                    var domainBill = await BillService.CreateDomainBill(booking.BillInfo, booking.Basket);

                    var route = await RouteService.GetRoute(booking.RootAddress, booking.Waypoints);
                    var domainRoute = await RouteService.CreateDomainRoute(route);

                    var result = await DomainOrderService.Create(
                        booking.OrderTime,
                        domainCustomer.Id,
                        domainCargo.Id,
                        domainRoute.Id,
                        domainBill.Id);

                    await OrderStateService.New(result.Id);

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

        public async Task<ICollection<OrderGroupAM>> GetOrderGroupsByStatuses(OrderStatus[] statuses)
        {
            var result = new List<OrderGroupAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            await statuses.ParallelForEachAsync(
                async status =>
                {
                    try
                    {
                        var stateOrderCount = await OrderStateService.GetCountByCurrentStatus(status);
                        var group = new OrderGroupAM { Status = status, Count = stateOrderCount };

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

            result = result.OrderBy(o => Array.IndexOf(statuses, o.Status)).ToList();

            return result;
        }

        public async Task<ICollection<Order>> GetDomainOrdersByStatus(OrderStatus status)
        {
            var ordersStates = await OrderStateService.GetByCurrentStatus(status);
            var ordersIdArray = ordersStates.Select(s => s.OrderId).ToArray();

            return await DomainOrderService.Get(ordersIdArray);
        }

        public async Task<ICollection<OrderAM>> GetOrdersByStatus(OrderStatus status)
        {
            var result = new List<OrderAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            var domainOrders = await GetDomainOrdersByStatus(status);

            await domainOrders.ParallelForEachAsync(
                async domainOrder =>
                {
                    try
                    {
                        //var cargo = await CargoService.GetEntity(domainOrder.CargoId);
                        var routeTitle = await RouteService.GetShortTitle(domainOrder.RouteId);

                        var ApplicationOrder = new OrderAM
                        {
                            ReceiptTime = domainOrder.AddedDate,
                            Id = domainOrder.Id,
                            Title = routeTitle
                        };

                        result.Add(ApplicationOrder);
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

            result = result.OrderBy(o => o.ReceiptTime).ToList();

            return result;
        }
    }
}