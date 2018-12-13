using Moq;
using System.Collections.Generic;
using TransportSystems.Backend.Application.Business.Ordering;
using TransportSystems.Backend.Application.Interfaces.Ordering;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Ordering
{
    public class ApplicationOrderStateServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationOrderStateServiceTestSuite()
        {
            DomainOrderStateServiceMock = new Mock<IOrderStateService>();
            DomainOrderServiceMock = new Mock<IOrderService>();

            OrderStateService = new ApplicationOrderStateService(
                TransactionServiceMock.Object,
                DomainOrderStateServiceMock.Object,
                DomainOrderServiceMock.Object);
        }

        public IApplicationOrderStateService OrderStateService { get; }

        public Mock<IOrderStateService> DomainOrderStateServiceMock { get; }

        public Mock<IOrderService> DomainOrderServiceMock { get; }
    }

    public class ApplicationOrderStateServiceTests : BaseServiceTests<ApplicationOrderStateServiceTestSuite>
    {
        [Fact]
        public async void New()
        {
            var orderId = 1;

            await Suite.OrderStateService.New(orderId);

            Suite.DomainOrderStateServiceMock
                .Verify(m => m.New(orderId));
        }

        [Fact]
        public async void Acept()
        {
            var commonId = 1;
            var orderId = commonId++;
            var moderatorId = commonId++;

            await Suite.OrderStateService.Accept(orderId, moderatorId);

            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Accept(orderId, moderatorId));
        }

        [Fact]
        public async void ReadyToTrade()
        {
            var commonId = 1;
            var orderId = commonId++;
            var moderatorId = commonId++;

            await Suite.OrderStateService.ReadyToTrade(orderId, moderatorId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ReadyToTrade(orderId, moderatorId));
        }

        [Fact]
        public async void Trade()
        {
            var commonId = 1;
            var orderId = commonId++;

            await Suite.OrderStateService.Trade(orderId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Trade(orderId));

        }

        [Fact]
        public async void AssignToDispatcher()
        {
            var commonId = 1;
            var orderId = commonId++;
            var dispatcherId = commonId++;

            await Suite.OrderStateService.AssignToDispatcher(orderId, dispatcherId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.AssignToDispatcher(orderId, dispatcherId));
        }

        [Fact]
        public async void AssignToDriver()
        {
            var commonId = 1;
            var orderId = commonId++;
            var dispatcherId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.AssignToDriver(orderId, dispatcherId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.AssignToDriver(orderId, dispatcherId, driverId));
        }

        [Fact]
        public async void ConfirmByDriver()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.ConfirmByDriver(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ConfirmByDriver(orderId, driverId));
        }

        [Fact]
        public async void GoToCustomer()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.GoToCustomer(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.GoToCustomer(orderId, driverId));
        }

        [Fact]
        public async void ArriveAtLoadingPlace()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.ArriveAtLoadingPlace(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ArriveAtLoadingPlace(orderId, driverId));
        }

        [Fact]
        public async void LoadTheVehicle()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.LoadTheVehicle(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.LoadTheVehicle(orderId, driverId));
        }

        [Fact]
        public async void DeliverTheVehicle()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.DeliverTheVehicle(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.DeliverTheVehicle(orderId, driverId));
        }

        [Fact]
        public async void ReceivePayment()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.ReceivePayment(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.ReceivePayment(orderId, driverId));
        }

        [Fact]
        public async void Complete()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.Complete(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Complete(orderId, driverId));
        }

        [Fact]
        public async void Cancel()
        {
            var commonId = 1;
            var orderId = commonId++;
            var driverId = commonId++;

            await Suite.OrderStateService.Cancel(orderId, driverId);
            Suite.DomainOrderStateServiceMock
                .Verify(m => m.Cancel(orderId, driverId));
        }

        [Fact]
        public async void GetByCurrentStatus()
        {
            var status = OrderStatus.Accepted;

            var states = new List<OrderState>();
            Suite.DomainOrderStateServiceMock
                .Setup(m => m.GetByCurrentStatus(status))
                .ReturnsAsync(states);

            var result = await Suite.OrderStateService.GetByCurrentStatus(status);

            Assert.Equal(states, result);
        }

        [Fact]
        public async void GetCountByCurrentStatus()
        {
            var status = OrderStatus.New;

            var count = 4;
            Suite.DomainOrderStateServiceMock
                .Setup(m => m.GetCountByCurrentStatus(status))
                .ReturnsAsync(count);

            var result = await Suite.OrderStateService.GetCountByCurrentStatus(status);
            Assert.Equal(count, result);
        }
    }
}