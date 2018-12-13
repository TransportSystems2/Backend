using System;

namespace TransportSystems.Backend.Core.Services.Interfaces
{
    public class EntityAlreadyExistsException : ArgumentException
    {
        public EntityAlreadyExistsException(string message) : base(message)
        {
        }

        public EntityAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}