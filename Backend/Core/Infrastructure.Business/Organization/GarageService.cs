using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces;
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
            bool isPublic,
            int companyId,
            int addressId,
            int pricelistId)
        {
            var garage = new Garage
            {
                IsPublic = isPublic,
                CompanyId = companyId,
                AddressId = addressId,
                PricelistId = pricelistId
            };

            await AddGarage(garage);

            return garage;
        }

        public async Task AssignPricelist(int garageId, int pricelistId)
        {
            if (!await PricelistService.IsExist(pricelistId))
            {
                throw new EntityNotFoundException($"PricelistId:{pricelistId} doesn't exist", "Pricelist");
            }

            var garage = await Get(garageId);
            if (garage == null)
            {
                throw new EntityNotFoundException($"GarageId:{garageId} doesn't exist", "Garage");
            }

            garage.PricelistId = pricelistId;

            await Repository.Update(garage);
            await Repository.Save();
        }

        protected async Task AddGarage(Garage garage)
        {
            await Verify(garage);

            await Repository.Add(garage);
            await Repository.Save();
        }

        public Task<ICollection<string>> GetAvailableProvinces(string country)
        {
            return AddressService.GetProvinces(AddressKind.Garage, country, OrderingKind.Asc);
        }

        public Task<ICollection<string>> GetAvailableLocalities(string country, string province)
        {
            return AddressService.GetLocalities(AddressKind.Garage, country, province, OrderingKind.Asc);
        }

        public Task<ICollection<string>> GetAvailableDistricts(string country, string province, string locality)
        {
            return AddressService.GetDistricts(AddressKind.Garage, country, province, locality, OrderingKind.Asc);
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
            var garageAddress = await AddressService.GetByCoordinate(latitude, longitude);

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

            if (!await PricelistService.IsExist(entity.PricelistId))
            {
                throw new EntityNotFoundException($"PricelistId:{entity.PricelistId} not found", "Pricelist");
            }

            return true;
        }
    }
}