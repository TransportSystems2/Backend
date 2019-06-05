using System;

namespace TransportSystems.Backend.Application.Models
{
    public class DomainAM : BaseAM
    {
        public DomainAM()
        {
        }

        public DomainAM(DomainAM source)
        {
            if (source != null)
            {
                Id = source.Id;
                AddedDate = source.AddedDate;
            }
        }

        public int Id { get; set; }

        public DateTime AddedDate { get; set; }
    }
}