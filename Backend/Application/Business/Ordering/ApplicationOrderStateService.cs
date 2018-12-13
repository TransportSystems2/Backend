using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;

namespace TransportSystems.Backend.Application.Business.Ordering
{
    public class ApplicationOrderStateService : ApplicationTransactionService, IApplicationOrderStateService
    {
        public ApplicationOrderStateService(
            ITransactionService transactionService,
            IOrderStateService domainOrderStateService,
            IOrderService domainOrderService)
            : base(transactionService)
        {
            DomainOrderStateService = domainOrderStateService;
            DomainOrderService = domainOrderService;
        }

        protected IOrderStateService DomainOrderStateService { get; }

        protected IOrderService DomainOrderService { get; }

        public Task New(int orderId)
        {
            return DomainOrderStateService.New(orderId);
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

        public Task<ICollection<OrderState>> GetByCurrentStatus(OrderStatus status)
        {
            return DomainOrderStateService.GetByCurrentStatus(status);
        }

        public Task<int> GetCountByCurrentStatus(OrderStatus status)
        {
            return DomainOrderStateService.GetCountByCurrentStatus(status);
        }
    }
}