using System;

namespace TransportSystems.Backend.Core.Domain.Core.Ordering
{
    public class OrderState : BaseEntity
    {
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime TimeOfDelivery { get; set; }          public int ModeratorId { get; set; }          public int CustomerId { get; set; }          public int PathId { get; set; }          public int RouteId { get; set; }          public int CargoId { get; set; }          public int CompanyId { get; set; }          public int DispatcherId { get; set; }          public int DriverId { get; set; }          public int BillId { get; set; }
    }
}