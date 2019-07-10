using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Application.Models.Catalogs;

namespace TransportSystems.Backend.Application.Interfaces.Catalogs
{
    public interface IApplicationCatalogService : IApplicationTransactionService
    {
        Task<ICollection<CatalogItemAM>> GetCatalogItems(CatalogKind catalogKind, CatalogItemKind catalogItemKind);

        Task<CatalogItemAM> CreateCatalogItem(int catalogId, CatalogItemAM item);

        Task<CatalogItemAM> GetCatalogItem(int itemId);
    }
}