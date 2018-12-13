using System;

namespace TransportSystems.Backend.Core.Domain.Core
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}