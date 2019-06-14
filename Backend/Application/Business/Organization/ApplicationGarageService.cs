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
            int companyId,
            AddressAM address)
        {
            var priceListBlank = await PricelistService.GetPricelistBlank();
            var domainPricelist = await PricelistService.CreateDomainPricelist(priceListBlank);

            var domainAddress = await AddressService.CreateDomainAddress(AddressKind.Garage, address);
            var result = await DomainGarageService.Create(
                companyId,
                domainAddress.Id);

            return result;
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
    }
}