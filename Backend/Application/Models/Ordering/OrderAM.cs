using System;

namespace TransportSystems.Backend.Application.Models.Ordering
{
    public class OrderAM : DomainAM
    {
        public string Title { get; set; }

        public DateTime ReceiptTime { get; set; }

        public decimal Cost { get; set; }

        public int CargoWeightKg { get; set; }
    }
}