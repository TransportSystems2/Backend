using System;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Interfaces.Exceptions
{
    public class ResponseStatusException : Exception
    {
        public ResponseStatusException(Status status)
            : this(status, string.Empty)
        {
        }

        public ResponseStatusException(Status status, string message)
            : base(message)
        {
        }
    }
}