using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business
{
    public class OrderService : DomainService<Order>, IOrderService
    {
        public OrderService(IOrderRepository repository,
            IModeratorService moderatorService,
            IDispatcherService dispatcherService,
            IDriverService driverService,
            ICustomerService customerService,
            ICargoService cargoService,
            IRouteService routeService,
            IBasketService billService)
            : base(repository)
        {
            ModeratorService = moderatorService;
            DispatcherService = dispatcherService;
            DriverService = driverService;
            CargoService = cargoService;
            CustomerService = customerService;
            RouteService = routeService;
            BillService = billService;
        }

        protected new IOrderRepository Repository => (IOrderRepository)base.Repository;

        protected IModeratorService ModeratorService { get; }

        protected IDispatcherService DispatcherService { get; }

        protected IDriverService DriverService { get; }

        protected ICustomerService CustomerService { get; }

        protected ICargoService CargoService { get; }

        protected IRouteService RouteService { get; }

        protected IBasketService BillService { get; }

        public async Task AssignModerator(int orderId, int moderatorId)
        {
            var order = await Get(orderId);

            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await ModeratorService.IsExist(moderatorId))
            {
                throw new EntityNotFoundException($"ModeratorId:{moderatorId} not found", "Moderator");
            }

            order.ModeratorId = moderatorId;
            await Repository.Update(order);
            await Repository.Save();
        }

        public async Task AssignDispatcher(int orderId, int dispatcherId)
        {
            var order = await Get(orderId);

            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            order.DispatcherId = dispatcherId;
            await Repository.Update(order);
            await Repository.Save();
        }

        public async Task AssignDriver(int orderId, int driverId)
        {
            var order = await Get(orderId);

            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            order.DriverId = driverId;
            await Repository.Update(order);
            await Repository.Save();
        }

        public async Task<Order> Create(
            DateTime time,
            int customerId,
            int cargoId,
            int routeId,
            int billId)
        {
            var order = new Order
            {
                Time = time,
                CustomerId = customerId,
                CargoId = cargoId,
                RouteId = routeId,
                BillId = billId
            };

            return await Create(order);
        }

        protected async Task<Order> Create(Order order)
        {
            await Verify(order);

            await Repository.Add(order);
            await Repository.Save();

            return order;
        }

        protected override async Task<bool> DoVerifyEntity(Order entity)
        {
            if (entity.Time == null)
            {
                throw new ArgumentNullException(nameof(entity.Time));
            }

            if (!await CustomerService.IsExist(entity.CustomerId))
            {
                throw new EntityNotFoundException($"CustomerId:{entity.CustomerId} not found", "Customer");
            }

            if (!await CargoService.IsExist(entity.CargoId))
            {
                throw new EntityNotFoundException($"GargoId:{entity.CargoId} not found", "Cargo");
            }

            if (!await RouteService.IsExist(entity.RouteId))
            {
                throw new EntityNotFoundException($"RouteId:{entity.RouteId} not found", "Route");
            }

            if (!await BillService.IsExist(entity.BillId))
            {
                throw new EntityNotFoundException($"BillId:{entity.BillId}", "Bill");
            }

            return true;
        }
    }
}