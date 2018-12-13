using System.Collections.Generic;

namespace TransportSystems.Backend.Application.Models.Common
{
    public class ApplicationResult<T>
    {
        public ApplicationResult()
        {
            Errors = new List<ApplicationError>();
            Succeeded = true;
        }

        public T Result { get; set; }

        public bool Succeeded { get; set; }

        public List<ApplicationError> Errors { get; }
    }
}