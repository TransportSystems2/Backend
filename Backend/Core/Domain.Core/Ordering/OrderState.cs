namespace TransportSystems.Backend.Core.Domain.Core.Ordering
{
    public class OrderState : BaseEntity
    {
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; }
    }
}