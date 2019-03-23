using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Business
{
    public class OrderService : DomainService<Order>, IOrderService
    {
        public OrderService(IOrderRepository repository)
            : base(repository)
        {
        }

        protected new IOrderRepository Repository => (IOrderRepository)base.Repository;

        public async Task<Order> Create()
        {
            var result = new Order();

            await Repository.Add(result);
            await Repository.Save();

            return result;
        }

        protected override Task<bool> DoVerifyEntity(Order entity)
        {
            return Task.FromResult(true);
        }
    }
}