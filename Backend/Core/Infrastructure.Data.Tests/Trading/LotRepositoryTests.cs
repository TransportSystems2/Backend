using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Domain.Interfaces.Trading;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Trading;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests.Trading
{
    public class LotRepositoryTests : BaseRepositoryTests<ILotRepository, Lot>
    {
        [Fact]
        public async Task GetLotByOrder()
        {
            var entities = new[]
            {
                new Lot { Id = 1, OrderId = 4, Status = LotStatus.Traded },
                new Lot { Id = 2, OrderId = 5, Status = LotStatus.Canceled },
                new Lot { Id = 3, OrderId = 6, Status = LotStatus.New }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByOrder(5);

            Assert.Equal(5, result.OrderId = 5);
        }

        [Fact]
        public async Task GetLotsByStatusWhereStatusIsTrade()
        {
            var entities = new[]
            {
                new Lot { Id = 1, OrderId = 4, Status = LotStatus.Traded },
                new Lot { Id = 2, OrderId = 5, Status = LotStatus.Canceled },
                new Lot { Id = 4, OrderId = 6, Status = LotStatus.New },
                new Lot { Id = 5, OrderId = 7, Status = LotStatus.New },
                new Lot { Id = 6, OrderId = 8, Status = LotStatus.New }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetByStatus(LotStatus.New);

            Assert.Equal(3, result.Count);
            foreach(var lot in result)
            {
                Assert.Equal(LotStatus.New, lot.Status);
            }
        }

        protected override ILotRepository CreateRepository(ApplicationContext context)
        {
            return new LotRepository(context);
        }
    }
}
