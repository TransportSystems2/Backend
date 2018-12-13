using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Catalogs
{
    public interface ICatalogRepository : IRepository<Catalog>
    {
        Task<Catalog> GetEntityByKind(CatalogKind kind);
    }
}