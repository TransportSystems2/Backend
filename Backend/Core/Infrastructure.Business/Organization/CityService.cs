using System;
using System.Threading.Tasks;
using Common.Extensions;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Geo;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Organization
{
    public class CityService :
        DomainService<City>,
        ICityService
    {
        public CityService(
            ICityRepository repository,
            IAddressService addressService,
            IPricelistService pricelistService)
            : base(repository)
        {
            AddressService = addressService;
            PricelistService = pricelistService;
        }

        protected new ICityRepository Repository => (ICityRepository)base.Repository;

        protected IAddressService AddressService { get; }

        protected IPricelistService PricelistService { get; }


        public async Task<City> Create(string domain, int addressId, int pricelistId)
        {
            var cityModel = new City
            {
                Domain = domain,
                AddressId = addressId,
                PricelistId = pricelistId
            };

            await Create(cityModel);

            return cityModel;
        }

        public async Task<City> GetByAddress(int addressId)
        {
            if (!await AddressService.IsExist(addressId))
            {
                throw new EntityNotFoundException($"AddressId:{addressId} doesn't exist", "Address"); 
            }

            return await Repository.GetByAddress(addressId);
        }

        public Task<bool> IsExistByDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException($"Argument: {nameof(domain)} can't be null or empty", nameof(domain).FirstCharToUpper());
            }

            return Repository.IsExistByDomain(domain);
        }

        protected async Task Create(City city)
        {
            await Verify(city);

            if (await Repository.GetByDomain(city.Domain) != null)
            {
                throw new EntityAlreadyExistsException($"City with domain:{city.Domain} is exist, cityId:{city.Id}", nameof(city.Domain));
            }

            await Repository.Add(city);
            await Repository.Save();
        }

        protected override async Task<bool> DoVerifyEntity(City entity)
        {
            if (string.IsNullOrEmpty(entity.Domain))
            {
                throw new ArgumentNullException("Domain");
            }

            if (!await AddressService.IsExist(entity.AddressId))
            {
                throw new EntityNotFoundException($"AddressId:{entity.AddressId} doesn't exist", "Address");
            }

            if (!await PricelistService.IsExist(entity.PricelistId))
            {
                throw new EntityNotFoundException($"PricelistId:{entity.PricelistId} doesn't exist", "Pricelist");
            }

            return true;
        }
    }
}