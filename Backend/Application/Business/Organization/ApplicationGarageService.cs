using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Business.Organization
{
    public class ApplicationGarageService : ApplicationTransactionService, IApplicationGarageService
    {
        public ApplicationGarageService(
            ITransactionService transactionService,
            IGarageService domainGarageService,
            IApplicationCityService cityService,
            IApplicationAddressService addressService)
            : base(transactionService)
        {
            DomainGarageService = domainGarageService;
            CityService = cityService;
            AddressService = addressService;
        }

        protected IGarageService DomainGarageService { get; }

        protected IApplicationCityService CityService { get; }

        protected IApplicationAddressService AddressService { get; }

        public async Task<Garage> CreateDomainGarage(
            bool isPublic,
            int companyId,
            int cityId,
            AddressAM address)
        {
            using (var transaction = await TransactionService.BeginTransaction())
            {
                try
                {
                    var domainCity = await CityService.GetDomainCity(cityId);
                    if (domainCity == null)
                    {
                        throw new EntityNotFoundException($"CityId: {cityId} doesn't exist.", "City");
                    }

                    var domainAddress = await AddressService.CreateDomainAddress(AddressKind.Garage, address);
                    var result = await DomainGarageService.Create(
                        isPublic,
                        companyId,
                        cityId,
                        domainAddress.Id,
                        domainCity.PricelistId);

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