using System;

namespace TransportSystems.Backend.Identity.Core.Domain.Data.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message)
            :base(message)
        {
        }
    }
}