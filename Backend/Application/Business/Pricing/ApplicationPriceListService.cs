using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Pricing;

namespace TransportSystems.Backend.Application.Business.Pricing
{
    public class ApplicationPricelistService : 
        ApplicationTransactionService,
        IApplicationPricelistService
    {
        public ApplicationPricelistService(
            ITransactionService transactionService,
            IPricelistService domainPricelistService,
            IPriceService domainPriceService,
            IApplicationCatalogService catalogService)
            : base(transactionService)
        {
            DomainPricelistService = domainPricelistService;
            DomainPriceService = domainPriceService;
            CatalogService = catalogService;
        }

        protected IPricelistService DomainPricelistService { get; }

        protected IPriceService DomainPriceService { get; }

        protected IApplicationCatalogService CatalogService { get; }

        public async Task<PricelistAM> GetPricelistBlank(
            CatalogKind catalogKind = CatalogKind.Cargo,
            CatalogItemKind catalogItemKind = CatalogItemKind.Weight,
            bool withDefaultPrices = true)
        {
            var catalogItems = await CatalogService.GetCatalogItems(catalogKind, catalogItemKind);

            var result = new PricelistAM();

            foreach (var catalogItem in catalogItems)
            {
                var price = new PriceAM
                {
                    CatalogItemId = catalogItem.Id,
                    CommissionPercentage = Price.DefaultComissionPercentage,
                    Name = catalogItem.Name
                };

                if (withDefaultPrices)
                {
                    FillPricesByDefault(price);
                }

                result.Items.Add(price);
            }

            return result;
        }

        public void FillPricesByDefault(PriceAM price)
        {
            price.PerMeter = Price.DefaultPerMeterPrice;
            price.Loading = Price.DefaultLoadingPrice;
            price.LockedSteering = Price.DefaultLockedSteeringPrice;
            price.LockedWheel = Price.DefaultLockedWheelPrice;
            price.Overturned = Price.DefaultOverturnedPrice;
            price.Ditch = Price.DefaultDitchPrice;
        }

        public async Task<Pricelist> CreateDomainPricelist(PricelistAM pricelist)
        {
            var result = await DomainPricelistService.Create();

            foreach (var price in pricelist.Items)
            {
                await CreateDomainPrice(result.Id, price);
            }

            return result;
        }

        public Task<Price> CreateDomainPrice(int pricelistId, PriceAM price)
        {
            return DomainPriceService.Create(
                pricelistId,
                price.CatalogItemId,
                price.Name,
                price.CommissionPercentage,
                price.PerMeter,
                price.Loading,
                price.LockedSteering,
                price.LockedWheel,
                price.Overturned,
                price.Ditch);
        }

        public Task<Price> GetDomainPrice(int priceId)
        {
            return DomainPriceService.Get(priceId);
        }
     
        public Task<Price> GetDomainPrice(int pricelistId, int catalogItemId)
        {
            return DomainPriceService.Get(pricelistId, catalogItemId);
        }
    }
}