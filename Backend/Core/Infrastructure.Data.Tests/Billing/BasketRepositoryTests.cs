using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Infrastructure.Database;
using TransportSystems.Backend.Core.Infrastructure.Database.Billing;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests.Billing
{
    public class BasketRepositoryTests : BaseRepositoryTests<IBasketRepository, Basket>
    {
        protected override IBasketRepository CreateRepository(ApplicationContext context)
        {
            return new BasketRepository(context);
        }
    }
}