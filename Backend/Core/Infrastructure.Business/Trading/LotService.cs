using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Trading
{
    public class LotService : DomainService<Lot>, ILotService
    {
        public LotService(
            ILotRepository repository, 
            IOrderStateService orderStateService, 
            IDispatcherService dispatcherService)
            : base(repository)
        {
            OrderStateService = orderStateService;
            DispatcherService = dispatcherService;
        }

        protected new ILotRepository Repository => (ILotRepository)base.Repository;

        protected IOrderStateService OrderStateService { get; }

        protected IDispatcherService DispatcherService { get; }

        public async Task<Lot> GetByOrder(int orderId)
        {
            var lot = await Repository.GetByOrder(orderId);

            if (lot == null)
            {
                lot = await Create(orderId);
            }

            return lot;
        }

        public Task<ICollection<Lot>> GetByStatus(LotStatus status)
        {
            return Repository.GetByStatus(status);
        }

        public async Task Trade(int lotId)
        {
            var lot = await Get(lotId);

            if (lot == null)
            {
                throw new EntityNotFoundException($"LotrId:{lotId} not found", "Lot");
            }

            if ((lot.Status != LotStatus.New) && (lot.Status != LotStatus.Expired))
            {
                throw new LotStatusException("Only active or expired lots can be traded");
            }

            await OrderStateService.Trade(lot.OrderId);

            lot.Status = LotStatus.Traded;
            await Repository.Update(lot);
            await Repository.Save();
        }

        public async Task Win(int lotId, int dispatcherId)
        {
            var lot = await Get(lotId);

            if (lot == null)
            {
                throw new EntityNotFoundException($"LotrId:{lotId} not found", "Lot");
            }

            if (lot.Status != LotStatus.Traded)
            {
                throw new LotStatusException("Only traded lots can be won");
            }

            if ((await OrderStateService.GetCurrentState(lot.OrderId)).Status != OrderStatus.SentToTrading)
            {
                throw new LotStatusException("Only traded orders can be won");
            }

            if (!await DispatcherService.IsExist(dispatcherId))
            {
                throw new EntityNotFoundException($"DispatcherId:{dispatcherId} not found", "Dispatcher");
            }

            lot.WinnerDispatcherId = dispatcherId;
            lot.Status = LotStatus.Won;
            await Repository.Update(lot);
            await Repository.Save();

            await OrderStateService.AssignToSubDispatcher(lot.OrderId, lot.WinnerDispatcherId);
        }

        public async Task Cancel(int lotId)
        {
            var lot = await Get(lotId);

            if (lot == null)
            {
                throw new EntityNotFoundException($"LotrId:{lotId} not found", "Lot");
            }

            if (lot.Status == LotStatus.Canceled)
            {
                throw new LotStatusException("Lot was already canceled");
            }

            lot.Status = LotStatus.Canceled;
            await Repository.Update(lot);
            await Repository.Save();
        }

        protected async Task<Lot> Create(int orderId)
        {
            var lot = new Lot { OrderId = orderId };

            await Verify(lot);

            if ((await OrderStateService.GetCurrentState(orderId)).Status != OrderStatus.ReadyForTrade)
            {
                throw new LotStatusException("Only for ready for trade orders can be created a lot");
            }

            lot.Status = LotStatus.New;

            await Repository.Add(lot);
            await Repository.Save();

            return lot;
        }

        protected override async Task<bool> DoVerifyEntity(Lot entity)
        {
            if (!await OrderStateService.IsExist(entity.OrderId))
            {
                throw new EntityNotFoundException($"OrderId:{entity.OrderId} not found", "Order");
            }

            return true;
        }
    }
}