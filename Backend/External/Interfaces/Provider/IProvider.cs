using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Interfaces.Provider
{
    public interface IProvider
    {
        ProviderKind Kind { get; }
    }
}