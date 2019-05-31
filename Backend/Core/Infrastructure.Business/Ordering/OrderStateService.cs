using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Routing;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business
{
    public class OrderStateService : DomainService<OrderState>, IOrderStateService
    {
        public OrderStateService(
            IOrderStateRepository repository,
            IOrderService orderService,
            IModeratorService moderatorService,
            IDispatcherService dispatcherService,
            IDriverService driverService,
            ICustomerService customerService,
            ICargoService cargoService,
            IRouteService routeService,
            IBillService billService,
            IMarketService marketService)
            : base(repository)
        {
            OrderService = orderService;
            ModeratorService = moderatorService;
            DispatcherService = dispatcherService;
            DriverService = driverService;
            CustomerService = customerService;
            CargoService = cargoService;
            RouteService = routeService;
            BillService = billService;
            MarketService = marketService;
        }

        protected new IOrderStateRepository Repository => (IOrderStateRepository)base.Repository;

        protected IOrderService OrderService { get; }

        protected IModeratorService ModeratorService { get; }

        protected IDispatcherService DispatcherService { get; }

        protected IDriverService DriverService { get; }

        protected ICustomerService CustomerService { get; }

        protected ICargoService CargoService { get; }

        protected IRouteService RouteService { get; }

        protected IBillService BillService { get; }

        protected IMarketService MarketService { get; }

        public async Task<OrderState> GetCurrentState(int orderId)
        {
            if (!await OrderService.IsExist(orderId))
            {
                throw new EntityNotFoundException($"OrderId:{orderId} doesn't exist", "Order");
            }

            return await Repository.GetCurrentState(orderId);
        }

        public Task<ICollection<OrderState>> GetByCurrentStatus(OrderStatus status)
        {
            return Repository.GetStatesByCurrentStatus(status);
        }

        public Task<int> GetCountByCurrentStatus(OrderStatus status)
        {
            return Repository.GetCountStatesByCurrentStatus(status);
        }

        public async Task New(
            int orderId,
            int marketId,
            DateTime timeOfDelivery,
            int customerId,
            int cargoId,
            int routeId,
            int billId)
        {
            var market = await MarketService.Get(marketId);
            if (market == null)
            {
                throw new ArgumentException($"MarketId:{cargoId} is null", "Market");
            }

            if (timeOfDelivery.ToUniversalTime() < DateTime.UtcNow)
            {
                throw new ArgumentException($"Time of delivery can't be lower that NowTime", nameof(timeOfDelivery).FirstCharToUpper());
            }

            var currentState = await GetCurrentState(orderId);
            if ((currentState != null) && (currentState.Status != OrderStatus.Canceled))
            {
                throw new OrderStatusException("Status the new can be set only to order without currentState or canceled status.");
            }

            if (timeOfDelivery == null)
            {
                throw new ArgumentNullException(nameof(timeOfDelivery).FirstCharToUpper());
            }

            if (!await CustomerService.IsExist(customerId))
            {
                throw new ArgumentException($"CustomerId:{customerId} doesn't exist", "Customer");
            }

            if (!await CargoService.IsExist(cargoId))
            {
                throw new ArgumentException($"CargoId:{cargoId} doesn't exist", "Cargo");
            }

            if (!await RouteService.IsExist(routeId))
            {
                throw new ArgumentException($"RouteId:{routeId} doesn't exist", "Route");
            }

            if (!await BillService.IsExist(billId))
            {
                throw new ArgumentException($"BillId:{billId} doesn't exist", "Bill");
            }

            if (currentState == null)
            {
                currentState = new OrderState { OrderId = orderId };
            }

            var newState = CreateState(currentState, OrderStatus.New);

            newState.GenCompanyId = market.CompanyId;
            newState.MarketId = marketId;
            newState.TimeOfDelivery = timeOfDelivery;
            newState.CustomerId = customerId;
            newState.CargoId = cargoId;
            newState.RouteId = routeId;
            newState.BillId = billId;

            await AddState(newState);
        }

        public async Task Accept(int orderId, int genDispatcherId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.New)
            {
                throw new OrderStatusException("Only new orders can be accepted");
            }

            var genDispatcher = await DispatcherService.Get(genDispatcherId);
            if (genDispatcher == null)
            {
                throw new EntityNotFoundException($"GenDispatcherId:{genDispatcherId} not found", "GenDispatcher");
            }

            if (!currentState.GenCompanyId.Equals(genDispatcher.CompanyId))
            {
                throw new ArgumentException($"CurrentState.GenCompanyId:{currentState.GenCompanyId} must be equal GenDispatcher.CompanyId:{genDispatcher.CompanyId}",
                "GenDispathcer");
            }

            var newState = CreateState(currentState, OrderStatus.Accepted);
            newState.GenDispatcherId = genDispatcherId;

            await AddState(newState);
        }

        public async Task ReadyToTrade(int orderId, int genDispatcherId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.Accepted)
            {
                throw new OrderStatusException("Only accepted orders can be read for trade");
            }

            if (!await DispatcherService.IsExist(genDispatcherId))
            {
                throw new EntityNotFoundException($"GenDispatcherId:{genDispatcherId} not found", "GenDispatcher");
            }

            if (!currentState.GenDispatcherId.Equals(genDispatcherId))
            {
                throw new AccessViolationException($"Only an genDispatcher can change the order state.  GenDispatcher:{currentState.GenDispatcherId}, function moderatorId:{genDispatcherId}");
            }

            var newState = CreateState(currentState, OrderStatus.ReadyForTrade);

            await AddState(newState);
        }

        public async Task Trade(int orderId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.ReadyForTrade)
            {
                throw new OrderStatusException("Only read for trade orders can be traded");
            }

            var newState = CreateState(currentState, OrderStatus.SentToTrading);

            await AddState(newState);
        }

        public async Task AssignToSubDispatcher(int orderId, int subDispatcherId)
        {
            var currentState = await GetCurrentState(orderId);
            if ((currentState.Status != OrderStatus.Accepted) && (currentState.Status != OrderStatus.SentToTrading))
            {
                throw new OrderStatusException("Only accepted or traded orders can be assigned to dispatcher");
            }

            var subDispatcher = await DispatcherService.Get(subDispatcherId);
            if (subDispatcher == null)
            {
                throw new EntityNotFoundException($"SubDispatcherId:{subDispatcherId} not found", "SubDispatcher");
            }

            var newState = CreateState(currentState, OrderStatus.AssignedDispatcher);
            newState.SubDispatcherId = subDispatcher.Id;
            newState.SubCompanyId = subDispatcher.CompanyId;

            await AddState(newState);
        }

        public async Task AssignToDriver(int orderId, int subDispatcherId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.AssignedDispatcher)
            {
                throw new OrderStatusException("Only assigned to dispatcher orders can be assigned to driver");
            }

            if (!await DispatcherService.IsExist(subDispatcherId))
            {
                throw new EntityNotFoundException($"SubDispatcherId:{subDispatcherId} not found", "SubDispatcher");
            }

            if (currentState.SubDispatcherId != subDispatcherId)
            {
                throw new AccessViolationException("Only a order subDispatcher can assign a order to driver");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            var newState = CreateState(currentState, OrderStatus.AssignedDriver);
            newState.DriverId = driverId;

            await AddState(newState);
        }

        public async Task ConfirmByDriver(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.AssignedDriver)
            {
                throw new OrderStatusException("Only AssignedDriver orders can be confirmed by driver");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only a order driver can confirm the order");
            }

            var newState = CreateState(currentState, OrderStatus.ConfirmedByDriver);

            await AddState(newState);
        }

        public async Task GoToCustomer(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.ConfirmedByDriver)
            {
                throw new OrderStatusException("Only ConfirmedByDriver orders can be gone to customer");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can go to customer");
            }

            var newState = CreateState(currentState, OrderStatus.WentToCustomer);

            await AddState(newState);
        }

        public async Task ArriveAtLoadingPlace(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.WentToCustomer)
            {
                throw new OrderStatusException("Only WentToCustomer orders can be arrived at loading place");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can arrive at loading place");
            }

            var newState = CreateState(currentState, OrderStatus.ArrivedAtLoadingPlace);

            await AddState(newState);
        }

        public async Task LoadTheVehicle(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.ArrivedAtLoadingPlace)
            {
                throw new OrderStatusException("Only ArrivedAtLoadingPlace orders can be loaded");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can load a vehicle");
            }

            var newState = CreateState(currentState, OrderStatus.VehicleIsLoaded);

            await AddState(newState);
        }

        public async Task DeliverTheVehicle(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.VehicleIsLoaded)
            {
                throw new OrderStatusException("Only VehicleIsLoaded orders can be delivered");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can deliver an vehicle");
            }

            var newState = CreateState(currentState, OrderStatus.VehicleIsDelivered);

            await AddState(newState);
        }

        public async Task ReceivePayment(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.VehicleIsDelivered)
            {
                throw new OrderStatusException("Only VehicleIsDelivered orders can be paymented");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can deliver an vehicle");
            }

            var newState = CreateState(currentState, OrderStatus.PaymentIsReceived);

            await AddState(newState);
        }

        public async Task Complete(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.PaymentIsReceived)
            {
                throw new OrderStatusException("Only paymend orders can be completed");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException("Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only owner driver can to complete order");
            }

            var newState = CreateState(currentState, OrderStatus.Completed);

            await AddState(newState);
        }

        public async Task Cancel(int orderId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (!OrderStatus.IsCarriedOut.HasFlag(currentState.Status))
            {
                throw new OrderStatusException("Only executing orders can be canceled");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException("Driver");
            }

            if (currentState.DriverId != driverId)
            {
                throw new AccessViolationException("Only owner driver can to cancel order");
            }

            var newState = CreateState(currentState, OrderStatus.AssignedDispatcher);
            newState.DriverId = 0;

            await AddState(newState);
        }

        protected async Task AddState(OrderState newState)
        {
            await Verify(newState);

            await Repository.Add(newState);
            await Repository.Save();
        }

        protected OrderState CreateState(OrderState currentState, OrderStatus status)
        {
            var result = (OrderState)currentState.Clone();
            result.Status = status;

            return result;
        }

        protected override async Task<bool> DoVerifyEntity(OrderState entity)
        {
            if (!await OrderService.IsExist(entity.OrderId))
            {
                throw new EntityNotFoundException($"OrderId:{entity.OrderId} doesn't exist", "Order");
            }

            return true;
        }
    }
}