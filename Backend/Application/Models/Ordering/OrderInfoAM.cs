using DotNetDistance;
using System;

namespace TransportSystems.Backend.Application.Models.Ordering
{
    public class OrderInfoAM : DomainAM
    {
        public string Title { get; set; }

        /// <summary>
        /// Time of delivery the vehicle to the customer.
        /// This time consists of the time of filing and the time of the preparation of the driver.
        /// </summary>
        public DateTime TimeOfDelivery { get; set; }

        public Distance Distance { get; set; }

        public decimal Cost { get; set; }
    }
}