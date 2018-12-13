using System;
using TransportSystems.Backend.External.Interfaces.Provider;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Business.Provider
{
    public class ProviderAccessor<T> : IProviderAccessor<T> where T : IProvider
    {
        public ProviderAccessor(Func<ProviderKind, T> accessor)
        {
            Accessor = accessor;
        }

        protected Func<ProviderKind, T> Accessor { get; }

        public T GetProvider(ProviderKind kind)
        {
            return Accessor(kind);
        }
    }
}
