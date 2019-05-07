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
            if (!await MarketService.IsExist(marketId))
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
                currentState = new OrderState { OrderId = orderId};
            }

            currentState.Status = OrderStatus.New;
            currentState.MarketId = marketId;
            currentState.TimeOfDelivery = timeOfDelivery;
            currentState.CustomerId = customerId;
            currentState.CargoId = cargoId;
            currentState.RouteId = routeId;
            currentState.BillId = billId;

            await AddNewState(currentState);
        }

        public async Task Accept(int orderId, int moderatorId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.New)
            {
                throw new OrderStatusException("Only new orders can be accepted");
            }

            if (!await ModeratorService.IsExist(moderatorId))
            {
                throw new EntityNotFoundException($"ModeratorId:{moderatorId} not found", "Moderator");
            }

            currentState.Status = OrderStatus.Accepted;
            currentState.ModeratorId = moderatorId;

            await AddNewState(currentState);
        }

        public async Task ReadyToTrade(int orderId, int moderatorId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.Accepted)
            {
                throw new OrderStatusException("Only accepted orders can be read for trade");
            }

            if (!await ModeratorService.IsExist(moderatorId))
            {
                throw new EntityNotFoundException($"ModeratorId:{moderatorId} not found", "Moderator");
            }

            if (!currentState.ModeratorId.Equals(moderatorId))
            {
                throw new AccessViolationException($"Only an order moderator can change the order state. Order moderatorId:{currentState.ModeratorId}, function moderatorId:{moderatorId}");
            }

            currentState.Status = OrderStatus.ReadyForTrade;
            await AddNewState(currentState);
        } 

        public async Task Trade(int orderId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.ReadyForTrade)
            {
                throw new OrderStatusException("Only read for trade orders can be traded");
            }

            currentState.Status = OrderStatus.SentToTrading;
            await AddNewState(currentState);
        }

        public async Task AssignToDispatcher(int orderId, int dispatcherId)
        {
            var currentState = await GetCurrentState(orderId);
            if ((currentState.Status != OrderStatus.Accepted) && (currentState.Status != OrderStatus.SentToTrading))
            {
                throw new OrderStatusException("Only accepted or traded orders can be assigned to dispatcher");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            currentState.Status = OrderStatus.AssignedDispatcher;
            currentState.DispatcherId = dispatcherId;
            await AddNewState(currentState);
        }

        public async Task AssignToDriver(int orderId, int dispatcherId, int driverId)
        {
            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.AssignedDispatcher)
            {
                throw new OrderStatusException("Only assigned to dispatcher orders can be assigned to driver");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            if (currentState.DispatcherId != dispatcherId)
            {
                throw new AccessViolationException("Only a order dispatcher can assign a order to driver");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            currentState.Status = OrderStatus.AssignedDriver;
            currentState.DriverId = driverId;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.ConfirmedByDriver;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.WentToCustomer;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.ArrivedAtLoadingPlace;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.VehicleIsLoaded;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.VehicleIsDelivered;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.PaymentIsReceived;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.Completed;
            await AddNewState(currentState);
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

            currentState.Status = OrderStatus.AssignedDispatcher;
            currentState.DriverId = 0;
            await AddNewState(currentState);
        }

        protected async Task AddNewState(OrderState newState)
        {
            await Verify(newState);

            await Repository.Add(newState);
            await Repository.Save();
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