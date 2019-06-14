using System.Linq;
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
    public class GarageService : DomainService<Garage>, IGarageService
    {
        public GarageService(IGarageRepository repository,
            ICompanyService companyService,
            IAddressService addressService,
            IPricelistService pricelistService)
            : base(repository)
        {
            CompanyService = companyService;
            AddressService = addressService;
            PricelistService = pricelistService;
        }

        protected new IGarageRepository Repository => (IGarageRepository)base.Repository;

        protected ICompanyService CompanyService { get; }

        protected IAddressService AddressService { get; }

        protected IPricelistService PricelistService { get; }

        public async Task<Garage> Create(
            int companyId,
            int addressId)
        {
            var garage = new Garage
            {
                CompanyId = companyId,
                AddressId = addressId,
            };

            await AddGarage(garage);

            return garage;
        }

        protected async Task AddGarage(Garage garage)
        {
            await Verify(garage);

            await Repository.Add(garage);
            await Repository.Save();
        }

        public async Task<Garage> GetByAddress(string country, string province, string locality, string district)
        {
            var garagesAddresses = await AddressService.GetByGeocoding(AddressKind.Garage, country, province, locality, district);
            var firstGarageAddress = garagesAddresses.FirstOrDefault();

            Garage result = null;
            if (firstGarageAddress != null)
            {
                result = await Repository.GetByAddress(firstGarageAddress.Id);
            }

            return result;
        }

        public async Task<Garage> GetByCoordinate(double latitude, double longitude)
        {
            var garageAddress = await AddressService.GetByCoordinate(AddressKind.Garage, latitude, longitude);

            Garage result = null;
            if (garageAddress != null)
            {
                result = await Repository.GetByAddress(garageAddress.Id);
            }

            return result;
        }

        protected override async Task<bool> DoVerifyEntity(Garage entity)
        {
            if (!await CompanyService.IsExist(entity.CompanyId))
            {
                throw new EntityNotFoundException($"CompanyId:{entity.CompanyId} not found", "Company");
            }

            if (!await AddressService.IsExist(entity.AddressId))
            {
                throw new EntityNotFoundException($"AddressId:{entity.AddressId} not found", "Address");
            }

            return true;
        }
    }
}