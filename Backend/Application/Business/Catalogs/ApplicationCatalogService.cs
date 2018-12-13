using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Mapping;

namespace TransportSystems.Backend.Application.Business.Catalogs
{
    public class ApplicationCatalogService :
        ApplicationTransactionService,
        IApplicationCatalogService
    {
        public ApplicationCatalogService(
            ITransactionService transactionService,
            IMappingService mappingService,
            ICatalogService domainCatalogService,
            ICatalogItemService domainCatalogItemService) 
            : base(transactionService)
        {
            MappingService = mappingService;

            DomainCatalogService = domainCatalogService;
            DomainCatalogItemService = domainCatalogItemService;
        }

        protected IMappingService MappingService { get; }

        protected ICatalogService DomainCatalogService { get; }

        protected ICatalogItemService DomainCatalogItemService { get; }

        public async Task<CatalogItemAM> CreateCatalogItem(int catalogId, CatalogItemAM item)
        {
            var domainResult = await DomainCatalogItemService.Create(catalogId, item.Kind, item.Name, item.Value);

            return FromDomainEntity(domainResult);
        }

        public async Task<List<CatalogItemAM>> GetCatalogItems(CatalogKind catalogKind, CatalogItemKind catalogItemKind)
        {
            var domainItems = await DomainCatalogItemService.GetByKind(catalogKind, catalogItemKind);

            var result = new List<CatalogItemAM>();
            foreach(var domainItem in domainItems)
            {
                result.Add(FromDomainEntity(domainItem));
            }

            return result;
        }

        private CatalogItemAM FromDomainEntity(CatalogItem source)
        {
            var destination = new CatalogItemAM();
            return MappingService.Map(source, destination);
        }
    }
}