using System;
using System.Threading.Tasks;
using Common.Models.Units;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Business.Organization
{
    public class ApplicationCityService :
        ApplicationTransactionService,
        IApplicationCityService
    {
        public ApplicationCityService(
            ITransactionService transactionService,
            ICityService domainCityService,
            IApplicationAddressService addressService,
            IApplicationPricelistService pricelistService)
            : base(transactionService)
        {
            DomainCityService = domainCityService;
            AddressService = addressService;
            PricelistService = pricelistService;
        }

        protected ICityService DomainCityService { get; }

        protected IApplicationAddressService AddressService { get; }

        protected IApplicationPricelistService PricelistService { get; }

        public async Task<City> CreateDomainCity(string domain, AddressAM address)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainAddress = await AddressService.CreateDomainAddress(AddressKind.City, address);
                    var priceListBlank = await PricelistService.GetPricelistBlank();
                    var domainPricelist = await PricelistService.CreateDomainPricelist(priceListBlank);
                    var domainCity = await DomainCityService.Create(domain, domainAddress.Id, domainPricelist.Id);

                    transaction.Commit();
                    return domainCity;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Task<City> GetDomainCity(int cityId)
        {
            return DomainCityService.Get(cityId);
        }

        public async Task<City> GetDomainCityByCoordinate(Coordinate coordinate)
        {
            var domainAddress = await AddressService.GetDomainAddressByCoordinate(coordinate);

            return await DomainCityService.GetByAddress(domainAddress.Id);
        }

        public Task<bool> IsExistByDomain(string domain)
        {
            return DomainCityService.IsExistByDomain(domain);
        }
    }
}