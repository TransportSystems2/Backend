using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Interfaces.Provider
{
    public interface IProviderAccessor<T> where T : IProvider
    {
        T GetProvider(ProviderKind kind);
    }
}