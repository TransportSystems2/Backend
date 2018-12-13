using System;
using TransportSystems.Backend.External.Models.Enums;

namespace TransportSystems.Backend.External.Interfaces.Exceptions
{
    public class DirectionException : Exception
    {
        public DirectionException(Status status, string message)
            : base(message)
        {
        }
    }
}