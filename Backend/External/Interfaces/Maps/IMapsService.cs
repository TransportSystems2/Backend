using Common.Models;
using Common.Models.Units;
using System.Threading.Tasks;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Interfaces.Services.Maps
{
    public interface IMapsService
    {
        ProviderKind[] DefaultProvidersKind { get; }

        Task<TimeBelt> GetTimeBelt(Coordinate coordinate, params ProviderKind[] providersKind);
    }
}