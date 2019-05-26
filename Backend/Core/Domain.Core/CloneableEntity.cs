using System;
using TransportSystems.Backend.Core.Domain.Core;

namespace Domain.Core
{
    public abstract class CloneableEntity : BaseEntity, ICloneable
    {
        public abstract object Clone();
    }
}