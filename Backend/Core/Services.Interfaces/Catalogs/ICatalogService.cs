using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;

namespace TransportSystems.Backend.Core.Services.Interfaces.Catalogs
{
    public interface ICatalogService : IDomainService<Catalog>
    {
        Task<Catalog> Create(CatalogKind catalogKind);

        Task<Catalog> GetByKind(CatalogKind kind);

        Task<bool> IsExist(CatalogKind kind);
    }
}