using Moq;

using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;
using TransportSystems.Backend.Core.Infrastructure.Business.Trading;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

using Xunit;

namespace TransportSystems.Infrastructure.Business.Tests.Trading
{
    public class LotRequestServiceTestSuite
    {
        public LotRequestServiceTestSuite()
        {
            LotRequestRepositoryMock = new Mock<ILotRequestRepository>();
            DispatcherServiceMock = new Mock<IDispatcherService>();
            LotServiceMock = new Mock<ILotService>();

            LotRequestService = new LotRequestService(LotRequestRepositoryMock.Object, LotServiceMock.Object, DispatcherServiceMock.Object);
        }

        public Mock<ILotRequestRepository> LotRequestRepositoryMock { get; }

        public Mock<ILotService> LotServiceMock { get; }

        public Mock<IDispatcherService> DispatcherServiceMock { get; }

        public ILotRequestService LotRequestService { get; }
    }

    public class LotRequestServiceTests
    {
        [Fact]
        public async void BetResultStatusBet()
        {
            var suite = new LotRequestServiceTestSuite();

            suite.LotServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);

            var lotRequest = await suite.LotRequestService.Bet(1, 1);

            Assert.Equal(LotRequestStatus.Bet, lotRequest.Status);
        }

        [Fact]
        public async void BetResultTheSameLot()
        {
            var suite = new LotRequestServiceTestSuite();
            var lot = new Lot { Id = 1 };

            suite.LotServiceMock
                .Setup(m => m.Get(lot.Id))
                .ReturnsAsync(lot);
            suite.LotServiceMock
                .Setup(m => m.IsExist(lot.Id))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);

            var lotRequest = await suite.LotRequestService.Bet(lot.Id, 1);

            Assert.Equal(lot.Id, lotRequest.LotId);
        }

        [Fact]
        public async void BetResultTheSameDispatcher()
        {
            var suite = new LotRequestServiceTestSuite();
            var dispatcher = new Dispatcher { Id = 1 };
            suite.LotServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
            .Setup(m => m.IsExist(dispatcher.Id))
            .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.Get(dispatcher.Id))
                .ReturnsAsync(dispatcher);

            var lotRequest = await suite.LotRequestService.Bet(1, dispatcher.Id);

            Assert.Equal(dispatcher.Id, lotRequest.DispatcherId);
        }

        [Fact]
        public async void BetWithoutLotResultLotNotFoundException()
        {
            var suite = new LotRequestServiceTestSuite();

            suite.LotServiceMock
                .Setup(m => m.IsExist(0))
                .ReturnsAsync(false);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityNotFoundException>("Lot", () => suite.LotRequestService.Bet(0, 1));
        }

        [Fact]
        public async void BetWithoutDispatcherResultDispatcherNotFoundException()
        {
            var suite = new LotRequestServiceTestSuite();
            suite.LotServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(0))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Dispatcher", () => suite.LotRequestService.Bet(1, 0));
        }

        [Fact]
        public async void GetRequestsByDispatcherResultDispatcherRequests()
        {
            var suite = new LotRequestServiceTestSuite();

            var dispatcherId = 1;

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcherId))
                .ReturnsAsync(true);
            
            var dispatcherRequests = await suite.LotRequestService.GetDispatcherRequests(dispatcherId);

            suite.LotRequestRepositoryMock
                .Verify(m => m.GetByDispatcher(dispatcherId));
        }

        [Fact]
        public async void GetRequestsByDispatcherResultDispatcherNotFoundException()
        {
            var suite = new LotRequestServiceTestSuite();

            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(It.IsAny<int>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Dispatcher", () => suite.LotRequestService.GetDispatcherRequests(0));
        }

        [Fact]
        public async void GetRequestsByLotResultLotRequests()
        {
            var suite = new LotRequestServiceTestSuite();

            var lotId = 1;

            suite.LotServiceMock
                .Setup(m => m.IsExist(lotId))
                .ReturnsAsync(true);

            var lotRequests = await suite.LotRequestService.GetLotRequests(lotId);

            suite.LotRequestRepositoryMock
                .Verify(m => m.GetByLot(lotId));
        }

        [Fact]
        public async void GetRequestsByLotResultLotNotFoundException()
        {
            var suite = new LotRequestServiceTestSuite();
            suite.LotServiceMock
                .Setup(m => m.IsExist(0))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Lot", () => suite.LotRequestService.GetLotRequests(0));
        }

        [Fact]
        public async void IsExistBetResultBetIsExist()
        {
            var suite = new LotRequestServiceTestSuite();

            var lotId = 1;
            var dispatcherId = 2;
            var lotRequest = new LotRequest { Id = 3, LotId = lotId, DispatcherId = dispatcherId, Status = LotRequestStatus.Bet };
            suite.LotServiceMock
                .Setup(m => m.IsExist(lotId))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcherId))
                .ReturnsAsync(true);
            suite.LotRequestRepositoryMock
                .Setup(m => m.GetCurrent(lotId, dispatcherId))
                .ReturnsAsync(lotRequest);

            Assert.True(await suite.LotRequestService.IsExistBet(lotId, dispatcherId));

            suite.LotRequestRepositoryMock
                .Verify(m => m.GetCurrent(lotId, dispatcherId));
        }

        [Fact]
        public async void CancelResultCanceledStatus()
        {
            var suite = new LotRequestServiceTestSuite();

            var betRequest = new LotRequest { Id = 1, LotId = 1, DispatcherId = 1, Status = LotRequestStatus.Bet };

            suite.LotServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.LotRequestRepositoryMock
                .Setup(m => m.GetCurrent(1, 1))
                .ReturnsAsync(betRequest);

            var canceledRequest = await suite.LotRequestService.Cancel(1, 1);

            Assert.Equal(LotRequestStatus.Canceled, canceledRequest.Status);
        }

        [Fact]
        public async void CancelResultNewEntity()
        {
            var suite = new LotRequestServiceTestSuite();

            var lotId = 1;
            var dispatcherId = 2;
            var currentLotRequest = new LotRequest { Id = 1, LotId = lotId, DispatcherId = dispatcherId, Status = LotRequestStatus.Bet };

            suite.LotRequestRepositoryMock
                .Setup(m => m.GetCurrent(lotId, dispatcherId))
                .ReturnsAsync(currentLotRequest);
            suite.LotServiceMock
                .Setup(m => m.IsExist(lotId))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(dispatcherId))
                .ReturnsAsync(true);

            var canceledRequest = await suite.LotRequestService.Cancel(lotId, dispatcherId);

            suite.LotRequestRepositoryMock
                .Verify(m =>
                    m.Add(It.Is<LotRequest>(
                        r => r.LotId.Equals(lotId)
                        && r.DispatcherId.Equals(dispatcherId)
                        && r.Status.Equals(LotRequestStatus.Canceled))));
        }

        [Fact]
        public async void CancelWithoutBetResultLotRequestStatusException()
        {
            var suite = new LotRequestServiceTestSuite();
            suite.LotRequestRepositoryMock
                .Setup(m => m.GetCurrent(1, 1))
                .Returns(Task.FromResult<LotRequest>(null));

            suite.LotServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<LotRequestStatusException>(() => suite.LotRequestService.Cancel(1, 1));
        }

        [Fact]
        public async void CancelNotOwnerDispatcherResultAccessViolationException()
        {
            var suite = new LotRequestServiceTestSuite();

            var betRequest = new LotRequest { Id = 1, LotId = 1, DispatcherId = 1, Status = LotRequestStatus.Bet };

            suite.LotServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(2))
                .ReturnsAsync(true);
            suite.LotRequestRepositoryMock
                .Setup(m => m.GetCurrent(1, 2))
                .ReturnsAsync(betRequest);

            await Assert.ThrowsAsync<AccessViolationException>(() => suite.LotRequestService.Cancel(1, 2));
        }

        [Fact]
        public async void CancelNotExistDispatcherResultDispatcherNotFoundException()
        {
            var suite = new LotRequestServiceTestSuite();
            suite.LotServiceMock
                .Setup(m => m.IsExist(1))
                .ReturnsAsync(true);
            suite.DispatcherServiceMock
                .Setup(m => m.IsExist(0))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Dispatcher", () => suite.LotRequestService.Cancel(1, 0));
        }

        [Fact]
        public async void CancelNullLotRequestResultLotNotFoundException()
        {
            var suite = new LotRequestServiceTestSuite();

            await Assert.ThrowsAsync<EntityNotFoundException>("Lot", () => suite.LotRequestService.Cancel(0, 1));
        }
    }
}
