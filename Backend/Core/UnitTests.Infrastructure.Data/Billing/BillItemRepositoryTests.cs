using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Billing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Data.Billing
{
    public class BillItemRepositoryTests : BaseRepositoryTests<IBillItemRepository, BillItem>
    {
        [Fact]
        public async Task GetTotalCost()
        {
            var entities = new List<BillItem>
            {
                new BillItem { Id = 1, BillId = 1, Cost = 123.4m },
                new BillItem { Id = 2, BillId = 1, Cost = 234.5m },
                new BillItem { Id = 3, BillId = 1, Cost = 345.6m },
                new BillItem { Id = 4, BillId = 2, Cost = 123.4m },
                new BillItem { Id = 5, BillId = 2, Cost = 123.4m }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetTotalCost(1);

            var totalCost = 703.5m;

            Assert.Equal(totalCost, result);
        }

        protected override IBillItemRepository CreateRepository(ApplicationContext context)
        {
            return new BillItemRepository(context);
        }
    }
}