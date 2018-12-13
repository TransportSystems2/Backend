using System;

namespace TransportSystems.Backend.Core.Services.Interfaces.Trading
{
    public class LotStatusException : Exception
    {
        public LotStatusException(string message)
            : base(message)
        {
        }
    }
}