namespace TransportSystems.Backend.Core.Domain.Core.Trading
{
    public class Lot : BaseEntity
    {
        public int OrderId { get; set; }

        public int WinnerDispatcherId { get; set; }

        public virtual LotStatus Status { get; set; }
    }
}