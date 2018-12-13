using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;

namespace TransportSystems.Backend.Core.Services.Interfaces.Catalogs
{
    public interface ICatalogItemService : IDomainService<CatalogItem>
    {
        Task<CatalogItem> Create(int catalogId, CatalogItemKind itemKind, string name, int value);

        Task<ICollection<CatalogItem>> GetByKind(CatalogKind catalogKind, CatalogItemKind itemKind);
    }
}