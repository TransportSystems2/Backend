using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Interfaces.Catalogs;

namespace TransportSystems.Backend.Core.Infrastructure.Database
{
    public class CatalogItemRepository : Repository<CatalogItem>, ICatalogItemRepository
    {
        public CatalogItemRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<ICollection<CatalogItem>> GetByKind(int catalogId, CatalogItemKind kind)
        {
            return await Entities.Where(
                e => e.CatalogId.Equals(catalogId)
                && e.Kind.Equals(kind)).ToListAsync();
        }
    }
}