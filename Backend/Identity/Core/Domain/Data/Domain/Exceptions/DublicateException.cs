using System;

namespace TransportSystems.Backend.Identity.Core.Domain.Data.Domain.Exceptions
{
    public class DublicateException : Exception
    {
        public DublicateException()
            :base()
        {
        }

        public DublicateException(string message)
            :base(message)
        {
        }
    }
}