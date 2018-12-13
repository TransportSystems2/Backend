using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Interfaces.Catalogs;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Catalogs
{
    public class CatalogRepository : Repository<Catalog>, ICatalogRepository
    {
        public CatalogRepository(ApplicationContext context)
            : base(context)
        {
        }

        public Task<Catalog> GetEntityByKind(CatalogKind kind)
        {
            return Entities.SingleOrDefaultAsync(l => l.Kind.Equals(kind));
        }
    }
}