using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Mapping;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationCargoService : ApplicationTransactionService, IApplicationCargoService
    {
        public ApplicationCargoService(
            ITransactionService transactionService,
            IMappingService mappingService,
            ICargoService domainCargoService,
            IApplicationCatalogService catalogService,
            IRegistrationNumberService registrationNumberService)
            : base(transactionService)
        {
            MappingService = mappingService;
            DomainCargoService = domainCargoService;
            CatalogService = catalogService;
            RegistrationNumberService = registrationNumberService;
        }

        protected ICargoService DomainCargoService { get; }

        protected IApplicationCatalogService CatalogService { get; }

        protected IMappingService MappingService { get; }

        protected IRegistrationNumberService RegistrationNumberService { get; }

        public Task<Cargo> CreateDomainCargo(CargoAM cargo)
        {
            return DomainCargoService.Create(
                cargo.WeightCatalogItemId,
                cargo.KindCatalogItemId,
                cargo.BrandCatalogItemId,
                cargo.RegistrationNumber,
                cargo.Comment);
        }

        public async Task<CargoAM> GetCargo(int cargoId)
        {
            var domainCargo = await DomainCargoService.Get(cargoId);

            if (domainCargo == null)
            {
                return null;
            }

            var result = MappingService.Map(domainCargo, new CargoAM());

            return result;
        }

        public async Task<CargoCatalogItemsAM> GetCatalogItems()
        {
            var result = new CargoCatalogItemsAM();
            
            var brands = await CatalogService.GetCatalogItems(CatalogKind.Cargo, CatalogItemKind.Brand);
            result.Brands.AddRange(brands);

            var weights = await CatalogService.GetCatalogItems(CatalogKind.Cargo, CatalogItemKind.Weight);
            result.Weights.AddRange(weights);

            var kinds = await CatalogService.GetCatalogItems(CatalogKind.Cargo, CatalogItemKind.Kind);
            result.Kinds.AddRange(kinds);

            return result;
        }

        public Task<Cargo> GetDomainCargo(int cargoId)
        {
            return DomainCargoService.Get(cargoId);
        }

        public Task<bool> ValidRegistrationNumber(string registrationNumber)
        {
            return RegistrationNumberService.ValidRegistrationNumber(registrationNumber);
        }
    }
}