using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Domain.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Transport
{
    public class VehicleService : DomainService<Vehicle>, IVehicleService
    {
        const float MinTonCapacity = 0.5f;
        const float MaxTonCapacity = 15f;

        public VehicleService(
            IVehicleRepository repository,
            ICompanyService companyService,
            ICatalogItemService catalogItemService,
            IRegistrationNumberService registrationNumberService)
            : base(repository)
        {
            CompanyService = companyService;
            CatalogItemService = catalogItemService;
            RegistrationNumberService = registrationNumberService;
        }

        protected new IVehicleRepository Repository => (IVehicleRepository)base.Repository;
    
        protected ICompanyService CompanyService { get; }

        protected ICatalogItemService CatalogItemService { get; }

        protected IRegistrationNumberService RegistrationNumberService { get; }

        public Task<Vehicle> Create(
            int companyId,
            string registrationNumber,
            int brandCatalogItemId,
            int capacityCatalogItemId,
            int kindCatalogItemId)
        {
            var vehicle = new Vehicle
            {
                CompanyId = companyId,
                RegistrationNumber = registrationNumber,
                BrandCatalogItemId = brandCatalogItemId,
                CapacityCatalogItemId = capacityCatalogItemId,
                KindCatalogItemId = kindCatalogItemId
            };

            return Create(vehicle);
        }

        protected async Task<Vehicle> Create(Vehicle vehicle)
        {
            await Verify(vehicle);

            await Repository.Add(vehicle);
            await Repository.Save();

            return vehicle;
        }

        protected override async Task<bool> DoVerifyEntity(Vehicle entity)
        {
            if (!await CompanyService.IsExist(entity.CompanyId))
            {
                throw new EntityNotFoundException($"Company with id = {entity.CompanyId}, doesn't exist", "CompanyId");
            }

            if (!await RegistrationNumberService.ValidRegistrationNumber(entity.RegistrationNumber))
            {
                throw new ArgumentException($"Not valid registrationNumber = {entity.RegistrationNumber}", "RegistrationNumber");
            }

            await VerifyCatalogItem(CatalogItemKind.Brand, entity.BrandCatalogItemId);
            await VerifyCatalogItem(CatalogItemKind.Capacity, entity.CapacityCatalogItemId);
            await VerifyCatalogItem(CatalogItemKind.Kind, entity.KindCatalogItemId);
   
            return true;
        }

        private async Task VerifyCatalogItem(CatalogItemKind validCatalogItemKind, int catalogItemId)
        {
            var catalogItem = await CatalogItemService.Get(catalogItemId);
            if (catalogItem == null)
            {
                throw new EntityNotFoundException(
                    $"CatalogItemId:{catalogItemId} doesn't exist",
                    nameof(catalogItemId));
            }

            if (!catalogItem.Kind.Equals(validCatalogItemKind))
            {
                throw new ArgumentException(
                    $"CatalogItem can't be set because it has a different kind. Necessary kind:{validCatalogItemKind}, catalog item kind:{catalogItem.Kind}");
            }
        }
    }
}