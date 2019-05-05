using Common.Models.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;

namespace TransportSystems.Backend.Application.Business.Organization
{
    public class ApplicationMarketService : ApplicationTransactionService, IApplicationMarketService
    {
        public ApplicationMarketService(
            ITransactionService transactionService,
            IMarketService domainMarketService,
            ICompanyService domainCompanyService,
            IApplicationAddressService addressService,
            IApplicationPricelistService pricelistService)
            : base(transactionService)
        {
            DomainMarketService = domainMarketService;
            DomainCompanyService = domainCompanyService;
            AddressService = addressService;
            PricelistService = pricelistService;
        }

        protected IMarketService DomainMarketService { get; }

        protected ICompanyService DomainCompanyService { get; }

        protected IApplicationAddressService AddressService { get; }

        protected IApplicationPricelistService PricelistService { get; }

        public async Task<Market> CreateDomainMarket(
            int companyId,
            AddressAM address)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var priceListBlank = await PricelistService.GetPricelistBlank();
                    var domainPricelist = await PricelistService.CreateDomainPricelist(priceListBlank);

                    var domainAddress = await AddressService.CreateDomainAddress(AddressKind.Market, address);
                    var result = await DomainMarketService.Create(
                        companyId,
                        domainAddress.Id,
                        domainPricelist.Id);

                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Task<Market> GetDomainMarket(int marketId)
        {
            return DomainMarketService.Get(marketId);
        }

        public Task<Market> GetDomainMarketByCoordinate(Coordinate coordinate)
        {
            return DomainMarketService.GetByCoordinate(coordinate.Latitude, coordinate.Longitude);
        }

        public async Task<ICollection<Market>> GetNearestDomainMarkets(int companyId, Coordinate coordinate)
        {
            if (!await DomainCompanyService.IsExist(companyId))
            {
                throw new ArgumentException($"CompanyId:{companyId} is null", "Company");
            }

            var marketAddresses = await AddressService.GetNearestDomainAddresses(AddressKind.Market, coordinate, 500, 0);
            var domainMarkets = await DomainMarketService.GetByAddressIds(marketAddresses.Select(m => m.Id).ToList());
            var result = domainMarkets.Where(m => m.CompanyId.Equals(companyId)).ToList();

            return result;
        }
    }
}