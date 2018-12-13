using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Application.Models.Pricing;

namespace TransportSystems.Backend.Application.Interfaces.Pricing
{
    public interface IApplicationPricelistService : IApplicationTransactionService
    {
        Task<PricelistAM> CreatePricelistBlank(
            CatalogKind catalogKind = CatalogKind.Cargo,
            CatalogItemKind catalogItemKind = CatalogItemKind.Weight);

        Task<Pricelist> CreateDomainPricelist();

        Task<Price> CreateDomainPrice(int pricelistId, PriceAM price);

        Task<Price> GetDomainPrice(int pricelistId, int catalogItemId);

        Task<Price> GetDomainPrice(int priceId);
    }
}