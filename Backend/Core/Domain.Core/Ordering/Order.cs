using System;

namespace TransportSystems.Backend.Core.Domain.Core.Ordering {     public class Order : BaseEntity     {
        /// <summary>
        /// Time of delivery the vehicle to the customer.
        /// This time consists of the time of filing and the time of the preparation of the driver.
        /// </summary>         public DateTime TimeOfDelivery { get; set; }          public int ModeratorId { get; set; }          public int CustomerId { get; set; }          public int PathId { get; set; }          public int RouteId { get; set; }          public int CargoId { get; set; }          public int CompanyId { get; set; }          public int DispatcherId { get; set; }          public int DriverId { get; set; }          public int BillId { get; set; }     } }