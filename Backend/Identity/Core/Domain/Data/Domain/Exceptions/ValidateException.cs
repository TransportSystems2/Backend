using System;

namespace TransportSystems.Backend.Identity.Core.Domain.Data.Domain.Exceptions
{
    public class ValidateException : Exception
    {
        public ValidateException(string message)
        :base(message)
        {
        }
    }
}