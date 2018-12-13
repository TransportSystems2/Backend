using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Catalogs
{
    public interface ICatalogItemRepository : IRepository<CatalogItem>
    {
        Task<ICollection<CatalogItem>> GetByKind(int catalogId, CatalogItemKind kind);
    }
}