using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Mapping;
using TransportSystems.Backend.Application.Models.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;

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
            ICatalogItemService domainCatalogItemService) : base(transactionService)
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

            return MappingService.Map<CatalogItemAM>(domainResult);
        }

        public async Task<CatalogItemAM> GetCatalogItem(int catalogItemId)
        {
            var domainCatalogItem = await DomainCatalogItemService.Get(catalogItemId);

            if (domainCatalogItem == null)
            {
                throw new ArgumentException($"CatalogItemId:{catalogItemId} doesn't exist.", "CatalogItem");
            }

            return MappingService.Map<CatalogItemAM>(domainCatalogItem);
        }

        public async Task<ICollection<CatalogItemAM>> GetCatalogItems(CatalogKind catalogKind, CatalogItemKind catalogItemKind)
        {
            var domainItems = await DomainCatalogItemService.GetByKind(catalogKind, catalogItemKind);

            return MappingService.Map<ICollection<CatalogItemAM>>(domainItems);
        }
    }
}