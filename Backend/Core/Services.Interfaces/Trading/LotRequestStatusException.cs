using System;

namespace TransportSystems.Backend.Core.Services.Interfaces.Trading
{
    public class LotRequestStatusException : Exception
    {
        public LotRequestStatusException(string message)
            :base(message)
        {
        }
    }
}