using Common.Models.Units;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationMarketService : IApplicationTransactionService
    {
        Task<Market> CreateDomainMarket(int companyId, AddressAM address);

        Task<Market> GetDomainMarket(int marketId);

        Task<Market> GetDomainMarketByCoordinate(Coordinate coordinate);

        Task<ICollection<Market>> GetNearestDomainMarkets(int companyId, Coordinate coordinate);
    }
}