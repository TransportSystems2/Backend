using System;
using TransportSystems.Backend.External.Business.Provider;
using TransportSystems.Backend.External.Interfaces.Direction;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Business.Direction
{
    public class DirectionAccessor : ProviderAccessor<IDirection>, IDirectionAccessor
    {
        public DirectionAccessor(Func<ProviderKind, IDirection> accessor)
            : base(accessor)
        {
        }
    }
}