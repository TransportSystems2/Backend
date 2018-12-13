using System;

namespace TransportSystems.Backend.Core.Services.Interfaces.Ordering
{
    public class OrderStatusException : Exception
    {
        public OrderStatusException(string message)
            : base(message)
        {
        }
    }
}