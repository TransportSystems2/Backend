using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Trading;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests.Trading
{
    public class LotRequestRepositoryTests : BaseRepositoryTests<ILotRequestRepository, LotRequest>
    {
        [Fact]
        public async Task GetLotRequestsByDispatcher()
        {
            var dispatcherId = 1;
            var anotherDispatcherId = 2;

            var entities = new []
            {
                new LotRequest { Id = 1, DispatcherId = dispatcherId },
                new LotRequest { Id = 2, DispatcherId = dispatcherId },
                new LotRequest { Id = 3, DispatcherId = anotherDispatcherId }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByDispatcher(dispatcherId);

            Assert.Equal(2, result.Count);
            foreach(var lotRequest in result)
            {
                Assert.Equal(dispatcherId, lotRequest.DispatcherId);
            }
        }

        [Fact]
        public async Task GetLotRequestByLot()
        {
            var lotId = 1;
            var anotherLotId = 2;

            var entities = new []
            {
                new LotRequest { Id = 1, LotId = lotId },
                new LotRequest { Id = 2, LotId = lotId },
                new LotRequest { Id = 3, LotId = lotId },
                new LotRequest { Id = 4, LotId = anotherLotId }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByLot(lotId);

            Assert.Equal(3, result.Count);
            foreach (var lotRequest in result)
            {
                Assert.Equal(lotId, lotRequest.LotId);
            }
        }

        [Fact]
        public async Task GetCurrentLotRequest()
        {
            var lotId = 1;
            var anotherLotId = 2;
            var dispatcherId = 5;
            var anotherDispatcherId = 6;

            var entities = new []
            {
                new LotRequest { Id = 1, LotId = lotId, DispatcherId = dispatcherId, Status = LotRequestStatus.Bet },
                new LotRequest { Id = 2, LotId = lotId, DispatcherId = dispatcherId, Status = LotRequestStatus.Canceled },
                new LotRequest { Id = 3, LotId = lotId, DispatcherId = dispatcherId, Status = LotRequestStatus.Bet },
                new LotRequest { Id = 4, LotId = anotherLotId, DispatcherId = anotherDispatcherId, Status = LotRequestStatus.Bet }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetCurrent(lotId, dispatcherId);

            Assert.Equal(3, result.Id);
            Assert.Equal(LotRequestStatus.Bet, result.Status);
        }

        protected override ILotRequestRepository CreateRepository(ApplicationContext context)
        {
            return new LotRequestRepository(context);
        }
    }
}