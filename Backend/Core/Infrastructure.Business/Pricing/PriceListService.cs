using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Pricing
{
    public class PricelistService : DomainService<Pricelist>, IPricelistService
    {
        public PricelistService(IPricelistRepository repository)
            : base(repository)
        {
        }

        protected new IPricelistRepository Repository => (IPricelistRepository)base.Repository;

        public async Task<Pricelist> Create()
        {
            var result = new Pricelist();

            await Repository.Add(result);
            await Repository.Save();

            return result;
        }

        protected override Task<bool> DoVerifyEntity(Pricelist entity)
        {
            return Task.FromResult(true);
        }
    }
}
