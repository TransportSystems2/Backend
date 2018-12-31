using System;
using TransportSystems.Backend.External.Business.Provider;
using TransportSystems.Backend.External.Interfaces.Maps;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Business.Maps
{
    public class MapsAccessor : ProviderAccessor<IMaps>, IMapsAccessor
    {
        public MapsAccessor(Func<ProviderKind, IMaps> acceessor)
            : base(acceessor)
        {
        }
    }
}