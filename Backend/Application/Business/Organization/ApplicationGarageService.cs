using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using Common.Models.Units;

namespace TransportSystems.Backend.Application.Business.Organization
{
    public class ApplicationGarageService : ApplicationTransactionService, IApplicationGarageService
    {
        public ApplicationGarageService(
            ITransactionService transactionService,
            IGarageService domainGarageService,
            IApplicationAddressService addressService,
            IApplicationPricelistService pricelistService)
            : base(transactionService)
        {
            DomainGarageService = domainGarageService;
            AddressService = addressService;
            PricelistService = pricelistService;
        }

        protected IGarageService DomainGarageService { get; }

        protected IApplicationAddressService AddressService { get; }

        protected IApplicationPricelistService PricelistService { get; }

        public async Task<Garage> CreateDomainGarage(
            bool isPublic,
            int companyId,
            AddressAM address)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var priceListBlank = await PricelistService.GetPricelistBlank();
                    var domainPricelist = await PricelistService.CreateDomainPricelist(priceListBlank);

                    var domainAddress = await AddressService.CreateDomainAddress(AddressKind.Garage, address);
                    var result = await DomainGarageService.Create(
                        isPublic,
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

        public Task<Garage> GetDomainGarage(int garageId)
        {
            return DomainGarageService.Get(garageId);
        }

        public Task<Garage> GetDomainGarageByAddress(AddressAM address)
        {
            return DomainGarageService.GetByAddress(address.Country, address.Province, address.Locality, address.District);
        }

        public Task<Garage> GetDomainGarageByCoordinate(Coordinate coordinate)
        {
            return DomainGarageService.GetByCoordinate(coordinate.Latitude, coordinate.Longitude);
        }

        public Task<ICollection<string>> GetAvailableProvinces(string country)
        {
            return DomainGarageService.GetAvailableProvinces(country);
        }

        public Task<ICollection<string>> GetAvailableLocalities(string country, string province)
        {
            return DomainGarageService.GetAvailableLocalities(country, province);
        }

        public Task<ICollection<string>> GetAvailableDistricts(string country, string province, string locality)
        {
            return DomainGarageService.GetAvailableDistricts(country, province, locality);
        }
    }
}