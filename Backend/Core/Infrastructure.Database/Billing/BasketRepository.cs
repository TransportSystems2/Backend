using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Billing
{
    public class BasketRepository : Repository<Basket>, IBasketRepository
    {
        public BasketRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}