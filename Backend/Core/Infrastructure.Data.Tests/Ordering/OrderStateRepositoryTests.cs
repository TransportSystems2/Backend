using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Geo;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests.Ordering
{
    public class OrderStateRepositoryTests : BaseRepositoryTests<IOrderStateRepository, OrderState>
    {
        [Fact]
        public async Task GetCurrentOrderState()
        {
            var entities = new List<OrderState>
            {
                new OrderState { Id = 1, OrderId = 1, Status = OrderStatus.New },
                new OrderState { Id = 2, OrderId = 1, Status = OrderStatus.Accepted },
                new OrderState { Id = 3, OrderId = 2, Status = OrderStatus.New }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetCurrentState(1);

            Assert.Equal(OrderStatus.Accepted, result.Status);
        }

        [Fact]
        public async Task GetStatesByCurrentStatus()
        {
            var entities = new List<OrderState>
            {
                new OrderState { Id = 1, OrderId = 1, Status = OrderStatus.New },
                new OrderState { Id = 2, OrderId = 1, Status = OrderStatus.Accepted },
                new OrderState { Id = 3, OrderId = 2, Status = OrderStatus.New },
                new OrderState { Id = 4, OrderId = 2, Status = OrderStatus.Accepted },
                new OrderState { Id = 5, OrderId = 2, Status = OrderStatus.ReadyForTrade },
                new OrderState { Id = 6, OrderId = 3, Status = OrderStatus.New }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetStatesByCurrentStatus(OrderStatus.New);

            Assert.Equal(6, result.First().Id);
        }

        [Fact]
        public async Task GetCountStatesByCurrentStatus()
        {
            var entities = new List<OrderState>
            {
                new OrderState { Id = 1, OrderId = 1, Status = OrderStatus.New },
                new OrderState { Id = 2, OrderId = 1, Status = OrderStatus.Accepted },
                new OrderState { Id = 3, OrderId = 2, Status = OrderStatus.New },
                new OrderState { Id = 4, OrderId = 2, Status = OrderStatus.Accepted },
                new OrderState { Id = 5, OrderId = 3, Status = OrderStatus.New },
                new OrderState { Id = 6, OrderId = 3, Status = OrderStatus.Accepted },
                new OrderState { Id = 7, OrderId = 3, Status = OrderStatus.Canceled }
            };

            await Repository.AddRange(entities);
            await Repository.Save();

            var result = await Repository.GetCountStatesByCurrentStatus(OrderStatus.Accepted);

            Assert.Equal(2, result);
        }

        protected override IOrderStateRepository CreateRepository(ApplicationContext context)
        {
            return new OrderStateRepository(context);
        }
    }
}