using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Trading
{
    public class LotRequestService : DomainService<LotRequest>, ILotRequestService
    {
        public LotRequestService(ILotRequestRepository repository, ILotService lotService, IDispatcherService dispatcherService)
            : base(repository)
        {
            LotService = lotService;
            DispatcherService = dispatcherService;
        }

        protected new ILotRequestRepository Repository => (ILotRequestRepository)base.Repository;

        protected IOrderService OrderService { get; }

        protected ILotService LotService { get; }

        protected IDispatcherService DispatcherService { get; }

        public Task<LotRequest> Bet(int lotId, int dispatcherId)
        {
            var lotRequest = new LotRequest
            {
                Status = LotRequestStatus.Bet,
                LotId = lotId,
                DispatcherId = dispatcherId,
            };

            return Create(lotRequest);
        }

        public async Task<LotRequest> Cancel(int lotId, int dispatcherId)
        {
            var currentDispatcherRequest = await GetCurrentRequest(lotId, dispatcherId);

            if ((currentDispatcherRequest == null) || (!currentDispatcherRequest.Status.Equals(LotRequestStatus.Bet)))
            {
                throw new LotRequestStatusException("Canceled request can't be created because not find dispatcher bet");
            }

            if (!currentDispatcherRequest.DispatcherId.Equals(dispatcherId))
            {
                throw new AccessViolationException("Only owner dispatcher can to cancel request");
            }

            var lotRequest = new LotRequest
            {
                Status = LotRequestStatus.Canceled,
                LotId = lotId,
                DispatcherId = dispatcherId
            };

            return await Create(lotRequest);
        }

        public async Task<ICollection<LotRequest>> GetDispatcherRequests(int dispatcherId)
        {
            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            return await Repository.GetByDispatcher(dispatcherId);
        }

        public async Task<ICollection<LotRequest>> GetLotRequests(int lotId)
        {
            if (!await LotService.IsExist(lotId))
            {
                throw new EntityNotFoundException($"LotId:{lotId} not found", "Lot");
            }

            return await Repository.GetByLot(lotId);
        }

        public async Task<bool> IsExistBet(int lotId, int dispatcherId)
        {
            if (!await LotService.IsExist(lotId))
            {
                throw new EntityNotFoundException($"LotId:{lotId} not found", "Lot");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            var currentRequest = await GetCurrentRequest(lotId, dispatcherId);

            return currentRequest?.Status == LotRequestStatus.Bet;
        }

        public async Task<LotRequest> GetCurrentRequest(int lotId, int dispatcherId)
        {
            if (!await LotService.IsExist(lotId))
            {
                throw new EntityNotFoundException($"LotId:{lotId} not found", "Lot");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            return await Repository.GetCurrent(lotId, dispatcherId);
        }

        protected async Task<LotRequest> Create(LotRequest lotRequest)
        {
            await Verify(lotRequest);

            await Repository.Add(lotRequest);
            await Repository.Save();

            return lotRequest;
        }

        protected override async Task<bool> DoVerifyEntity(LotRequest entity)
        {
            if (!await LotService.IsExist(entity.LotId))
            {
                throw new EntityNotFoundException($"Owner LotId:{entity.LotId} not found", "Lot");
            }

            if (!await DispatcherService.IsExist(entity.DispatcherId))
            {
                throw new EntityNotFoundException($"Owner DispatcherId:{entity.DispatcherId} not found", "Dispatcher");
            }

            return true;
        }
    }
}