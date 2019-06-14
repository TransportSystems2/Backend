using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Organization
{
    public class MarketService : DomainService<Market>, IMarketService
    {
        public MarketService(
            IMarketRepository repository,
            ICompanyService companyService,
            IAddressService addressService,
            IPricelistService pricelistService)
            : base(repository)
        {
            CompanyService = companyService;
            AddressService = addressService;
            PricelistService = pricelistService;
        }

        protected new IMarketRepository Repository { get => (IMarketRepository)base.Repository; }

        protected ICompanyService CompanyService { get; }

        protected IAddressService AddressService { get; }

        protected IPricelistService PricelistService { get; }

        public async Task<Market> Create(
            int companyId,
            int addressId,
            int pricelistId)
        {
            var market = new Market
            {
                CompanyId = companyId,
                AddressId = addressId,
                PricelistId = pricelistId
            };

            await AddMarket(market);

            return market;
        }

        public async Task<Market> GetByCoordinate(double latitude, double longitude)
        {
            var address = await AddressService.GetByCoordinate(AddressKind.Market, latitude, longitude);

            Market result = null;
            if (address != null)
            {
                result = await Repository.GetByAddress(address.Id);
            }

            return result;
        }

        public Task<ICollection<Market>> GetByAddressIds(ICollection<int> addressIds)
        {
            return Repository.GetByAddressIds(addressIds);
        }

        protected async Task AddMarket(Market market)
        {
            await Verify(market);

            await Repository.Add(market);
            await Repository.Save();
        }

        protected override async Task<bool> DoVerifyEntity(Market entity)
        {
            if (!await CompanyService.IsExist(entity.CompanyId))
            {
                throw new EntityNotFoundException($"CompanyId:{entity.CompanyId} not found", "Company");
            }

            if (!await AddressService.IsExist(entity.AddressId))
            {
                throw new EntityNotFoundException($"AddressId:{entity.AddressId} not found", "Address");
            }

            if (!await PricelistService.IsExist(entity.PricelistId))
            {
                throw new EntityNotFoundException($"PricelistId:{entity.PricelistId} not found", "Pricelist");
            }

            return true;
        }
    }
}
