using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Domain.Interfaces.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Pricing
{
    public class PriceService : DomainService<Price>, IPriceService
    {
        public PriceService(
            IPriceRepository repository,
            IPricelistService pricelistService,
            ICatalogItemService catalogItemService)
            : base(repository)
        {
            PricelistService = pricelistService;
            CatalogItemService = catalogItemService;
        }

        protected new IPriceRepository Repository => (IPriceRepository)base.Repository;

        protected IPricelistService PricelistService { get; }

        protected ICatalogItemService CatalogItemService { get; }

        public async Task<Price> Create(
            int pricelistId,
            int catalogItemId,
            string name,
            byte commissionPercentage,
            decimal perKm,
            decimal loading,
            decimal lockedSteering,
            decimal lockedWheel,
            decimal overturned,
            decimal ditch)
        {
            var existingPrice = await Get(pricelistId, catalogItemId);
            if (existingPrice != null)
            {
                throw new EntityAlreadyExistsException($"Price with parameters pricelistId:{pricelistId}, catalogItemId:{catalogItemId} already exists. Existing priceId:{existingPrice.Id}", "Price");
            }

            var price = new Price
            {
                PricelistId = pricelistId,
                CatalogItemId = catalogItemId,
                Name = name,
                CommissionPercentage = commissionPercentage,
                PerKm = perKm,
                Loading = loading,
                LockedSteering = lockedSteering,
                LockedWheel = lockedWheel,
                Overturned = overturned,
                Ditch = ditch
            };

            return await Create(price);
        }

        public async Task<Price> Get(int pricelistId, int catalogItemId)
        {
            return await Repository.Get(pricelistId, catalogItemId);
        }

        protected async Task<Price> Create(Price price)
        {
            await Verify(price);

            await Repository.Add(price);
            await Repository.Save();

            return price;
        }

        protected override async Task<bool> DoVerifyEntity(Price entity)
        {
            if (!await PricelistService.IsExist(entity.PricelistId))
            {
                throw new EntityNotFoundException($"PricelistId:{entity.PricelistId} does not exist", "Pricelist");
            }

            if (!await CatalogItemService.IsExist(entity.CatalogItemId))
            {
                throw new EntityNotFoundException($"CatalogItemId:{entity.CatalogItemId} does not exist", "CatalogItem");
            }

            return true;
        }
    }
}