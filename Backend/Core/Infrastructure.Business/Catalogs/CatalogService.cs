using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Interfaces.Catalogs;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Catalogs;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Catalogs
{
    public class CatalogService : DomainService<Catalog>, ICatalogService
    {
        public CatalogService(ICatalogRepository repository)
            : base(repository)
        {
        }

        protected new ICatalogRepository Repository => base.Repository as ICatalogRepository;

        public Task<Catalog> Create(CatalogKind catalogKind)
        {
            var catalog = new Catalog
            {
                Kind = catalogKind
            };

            return Create(catalog);
        }

        public Task<Catalog> GetByKind(CatalogKind kind)
        {
            return Repository.GetEntityByKind(kind);
        }

        public async Task<bool> IsExist(CatalogKind kind)
        {
            return await GetByKind(kind) != null;
        }

        protected async Task<Catalog> Create(Catalog catalog)
        {
            await Verify(catalog);

            var existCatalog = await IsExist(catalog.Kind);
            if (existCatalog)
            {
                throw new EntityAlreadyExistsException($"catalog with kind: {catalog.Kind} alredy exist");
            }

            await Repository.Add(catalog);
            await Repository.Save();

            return catalog;
        }

        protected override Task<bool> DoVerifyEntity(Catalog entity)
        {
            if (entity == null)
            {
                throw new NullReferenceException("Catalog is null");
            }

            return Task.FromResult(true);
        }
    }
}