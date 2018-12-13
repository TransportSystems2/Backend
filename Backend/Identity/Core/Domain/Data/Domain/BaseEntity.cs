using System;

namespace TransportSystems.Backend.Identity.Core.Domain.Data.Domain
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}