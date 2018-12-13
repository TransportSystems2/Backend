using System;

namespace TransportSystems.Backend.Core.Services.Interfaces.Exceptions
{
    public class IdentityException : Exception
    {
        public IdentityException(string message)
            : base(message)
        {
        }
    }
}