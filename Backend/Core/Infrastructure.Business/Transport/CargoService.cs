using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Transport
{
    public class CargoService : DomainService<Cargo>, ICargoService
    {
        public CargoService(ICargoRepository repository)
            : base(repository)
        {
        }

        protected new ICargoRepository Repository => (ICargoRepository)base.Repository;

        public async Task<Cargo> Create(
            int weightCatalogItemId,
            int kindCatalogItemId,
            int brandCatalogItemId,
            string registrationNumber = null,
            string comment = null)
        {
            var result = new Cargo
            {
                WeightCatalogItemId = weightCatalogItemId,
                KindCatalogItemId = kindCatalogItemId,
                BrandCatalogItemId = brandCatalogItemId,
                RegistrationNumber = registrationNumber,
                Comment = comment
            };

            await Verify(result);

            await Repository.Add(result);
            await Repository.Save();

            return result;
        }

        protected override Task<bool> DoVerifyEntity(Cargo entity)
        {
            return Task.FromResult(true);
        }
    }
}