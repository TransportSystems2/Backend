using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
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
            IDriverService driverService)
            : base(repository)
        {
            OrderService = orderService;
            ModeratorService = moderatorService;
            DispatcherService = dispatcherService;
            DriverService = driverService;
        }

        protected new IOrderStateRepository Repository => (IOrderStateRepository)base.Repository;

        protected IOrderService OrderService { get; }

        protected IModeratorService ModeratorService { get; }

        protected IDispatcherService DispatcherService { get; }

        protected IDriverService DriverService { get; }

        public Task<OrderState> GetCurrentState(int orderId)
        {
            return Repository.GetCurrentState(orderId);
        }

        public Task<ICollection<OrderState>> GetByCurrentStatus(OrderStatus status)
        {
            return Repository.GetStatesByCurrentStatus(status);
        }

        public Task<int> GetCountByCurrentStatus(OrderStatus status)
        {
            return Repository.GetCountStatesByCurrentStatus(status);
        }

        public async Task New(int orderId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            var currentState = await GetCurrentState(orderId);
            if ((currentState != null) && (currentState.Status != OrderStatus.Canceled))
            {
                throw new OrderStatusException("Status the new can be set only to order without currentState or canceled status.");
            }

            await SetCurrentStatus(orderId, OrderStatus.New);
        }

        public async Task Accept(int orderId, int moderatorId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await ModeratorService.IsExist(moderatorId))
            {
                throw new EntityNotFoundException($"ModeratorId:{moderatorId} not found", "Moderator");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.New)
            {
                throw new OrderStatusException("Only new orders can be accepted");
            }

            await SetCurrentStatus(orderId, OrderStatus.Accepted);
            await OrderService.AssignModerator(orderId, moderatorId);
        }

        public async Task ReadyToTrade(int orderId, int moderatorId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.Accepted)
            {
                throw new OrderStatusException("Only accepted orders can be read for trade");
            }

            if (!await ModeratorService.IsExist(moderatorId))
            {
                throw new EntityNotFoundException($"ModeratorId:{moderatorId} not found", "Moderator");
            }

            if (!order.ModeratorId.Equals(moderatorId))
            {
                throw new AccessViolationException($"Only a order moderator can change the order state. Order moderatorId:{order.ModeratorId}, function moderatorId:{moderatorId}");
            }

            await SetCurrentStatus(orderId, OrderStatus.ReadyForTrade);
        } 

        public async Task Trade(int orderId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException("Order");
            }

            var currentState = await GetCurrentState(orderId);
            if (currentState.Status != OrderStatus.ReadyForTrade)
            {
                throw new OrderStatusException("Only read for trade orders can be traded");
            }

            await SetCurrentStatus(orderId, OrderStatus.SentToTrading);
        }

        public async Task AssignToDispatcher(int orderId, int dispatcherId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            var currentState = await GetCurrentState(orderId);
            if ((currentState.Status != OrderStatus.Accepted) && (currentState.Status != OrderStatus.SentToTrading))
            {
                throw new OrderStatusException("Only accepted or traded orders can be assigned to dispatcher");
            }

            await SetCurrentStatus(orderId, OrderStatus.AssignedDispatcher);
            await OrderService.AssignDispatcher(orderId, dispatcherId);
        }

        public async Task AssignToDriver(int orderId, int dispatcherId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            if (order.DispatcherId != dispatcherId)
            {
                throw new AccessViolationException("Only a order dispatcher can assign a order to driver");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.AssignedDispatcher)
            {
                throw new OrderStatusException("Only assigned to dispatcher orders can be assigned to driver");
            }

            await SetCurrentStatus(orderId, OrderStatus.AssignedDriver);
            await OrderService.AssignDriver(orderId, driverId);
        }

        public async Task ConfirmByDriver(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only a order driver can confirm the order");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.AssignedDriver)
            {
                throw new OrderStatusException("Only AssignedDriver orders can be confirmed by driver");
            }

            await SetCurrentStatus(orderId, OrderStatus.ConfirmedByDriver);
        }

        public async Task GoToCustomer(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can go to customer");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.ConfirmedByDriver)
            {
                throw new OrderStatusException("Only ConfirmedByDriver orders can be gone to customer");
            }

            await SetCurrentStatus(orderId, OrderStatus.WentToCustomer);
        }

        public async Task ArriveAtLoadingPlace(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can arrive at loading place");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.WentToCustomer)
            {
                throw new OrderStatusException("Only WentToCustomer orders can be arrived at loading place");
            }

            await SetCurrentStatus(orderId, OrderStatus.ArrivedAtLoadingPlace);
        }

        public async Task LoadTheVehicle(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can load a vehicle");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.ArrivedAtLoadingPlace)
            {
                throw new OrderStatusException("Only ArrivedAtLoadingPlace orders can be loaded");
            }

            await SetCurrentStatus(orderId, OrderStatus.VehicleIsLoaded);
        }

        public async Task DeliverTheVehicle(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can deliver an vehicle");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.VehicleIsLoaded)
            {
                throw new OrderStatusException("Only VehicleIsLoaded orders can be delivered");
            }

            await SetCurrentStatus(orderId, OrderStatus.VehicleIsDelivered);
        }

        public async Task ReceivePayment(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException($"OrderId:{orderId} not found", "Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException($"DriverId:{driverId} not found", "Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only an owner driver can deliver an vehicle");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.VehicleIsDelivered)
            {
                throw new OrderStatusException("Only VehicleIsDelivered orders can be paymented");
            }

            await SetCurrentStatus(orderId, OrderStatus.PaymentIsReceived);
        }

        public async Task Complete(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException("Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException("Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only owner driver can to complete order");
            }

            if ((await GetCurrentState(orderId)).Status != OrderStatus.PaymentIsReceived)
            {
                throw new OrderStatusException("Only paymend orders can be completed");
            }

            await SetCurrentStatus(orderId, OrderStatus.Completed);
        }

        public async Task Cancel(int orderId, int driverId)
        {
            var order = await OrderService.Get(orderId);
            if (order == null)
            {
                throw new EntityNotFoundException("Order");
            }

            if (!await DriverService.IsExist(driverId))
            {
                throw new EntityNotFoundException("Driver");
            }

            if (order.DriverId != driverId)
            {
                throw new AccessViolationException("Only owner driver can to cancel order");
            }

            var currentState = await GetCurrentState(orderId);
            if (!OrderStatus.IsCarriedOut.HasFlag(currentState.Status))
            {
                throw new OrderStatusException("Only executing orders can be canceled");
            }

            await SetCurrentStatus(orderId, OrderStatus.AssignedDispatcher);
        }

        protected async Task SetCurrentStatus(int orderId, OrderStatus status)
        {
            var state = new OrderState
            {
                OrderId = orderId,
                Status = status
            };

            await Repository.Add(state);
            await Repository.Save();
        }

        protected override Task<bool> DoVerifyEntity(OrderState entity)
        {
            return Task.FromResult(true);
        }
    }
}