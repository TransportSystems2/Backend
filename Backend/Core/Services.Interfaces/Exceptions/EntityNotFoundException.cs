using System;

namespace TransportSystems.Backend.Core.Services.Interfaces
{
    public class EntityNotFoundException : ArgumentException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}