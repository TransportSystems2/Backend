using System.Threading.Tasks;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationVehicleService :
        ApplicationTransactionService,
        IApplicationVehicleService
    {
        public ApplicationVehicleService(
            ITransactionService transactionService,
            IVehicleService domainVehicleService,
            IApplicationCatalogService catalogService)
            : base(transactionService)
        {
            DomainVehicleService = domainVehicleService;
            CatalogService = catalogService;
        }

        protected IVehicleService DomainVehicleService { get; }

        protected IApplicationCatalogService CatalogService { get; }

        public Task<Vehicle> CreateDomainVehicle(int companyId, VehicleAM vehicle)
        {
            return DomainVehicleService.Create(
                companyId,
                vehicle.RegistrationNumber,
                vehicle.BrandCatalogItemId,
                vehicle.CapacityCatalogItemId,
                vehicle.KindCatalogItemId);
        }

        public async Task<VehicleCatalogItemsAM> GetCatalogItems()
        {
            var result = new VehicleCatalogItemsAM();

            var brands = await CatalogService.GetCatalogItems(CatalogKind.Vehicle, CatalogItemKind.Brand);
            result.Brands.AddRange(brands);

            var capacities = await CatalogService.GetCatalogItems(CatalogKind.Vehicle, CatalogItemKind.Capacity);
            result.Capacities.AddRange(capacities);

            var kinds = await CatalogService.GetCatalogItems(CatalogKind.Vehicle, CatalogItemKind.Kind);
            result.Kinds.AddRange(kinds);

            return result;
        }
    }
}