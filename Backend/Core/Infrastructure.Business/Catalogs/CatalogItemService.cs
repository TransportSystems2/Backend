using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Catalogs
{
    public class CatalogItemService : DomainService<CatalogItem>, ICatalogItemService
    {
        public CatalogItemService(
            ICatalogItemRepository repository,
            ICatalogService catalogService)
            : base(repository)
        {
            CatalogService = catalogService;
        }

        protected ICatalogService CatalogService { get; }

        protected new ICatalogItemRepository Repository => base.Repository as ICatalogItemRepository;

        public async Task<CatalogItem> Create(int catalogId, CatalogItemKind itemKind, string name, int value)
        {
            var item = new CatalogItem
            {
                CatalogId = catalogId,
                Kind = itemKind,
                Name = name,
                Value = value
            };

            return await Create(item);
        }

        public async Task<ICollection<CatalogItem>> GetByKind(CatalogKind catalogKind, CatalogItemKind itemKind)
        {
            var catalog = await CatalogService.GetByKind(catalogKind);
            if (catalog == null)
            {
                throw new EntityNotFoundException(
                    $"can't find catalogItem with catalogKind:{catalogKind}", 
                    nameof(catalogKind));
            }

            return await Repository.GetByKind(catalog.Id, itemKind);
        }

        public async Task<CatalogItem> Remove(int id)
        {
            var item = await Get(id);
            if (item == null)
            {
                throw new EntityNotFoundException($"Not found entity with id: {id}", "id");
            }

            await Repository.Remove(item);
            await Repository.Save();

            return item;
        }

        protected async Task<CatalogItem> Create(CatalogItem item)
        {
            await Verify(item);

            await Repository.Add(item);
            await Repository.Save();

            return item;
        }

        protected override async Task<bool> DoVerifyEntity(CatalogItem entity)
        {
            if (!await CatalogService.IsExist(entity.CatalogId))
            {
                throw new EntityNotFoundException($"CatalogId:{entity.CatalogId} doesn't exist.", "Catalog");
            }

            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new ArgumentNullException("Name", "argument can't be null or empty");
            }

            return true;
        }
    }
}