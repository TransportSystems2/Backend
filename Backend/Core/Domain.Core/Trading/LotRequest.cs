namespace TransportSystems.Backend.Core.Domain.Core.Trading
{
    public class LotRequest : BaseEntity
    {
        public LotRequest()
        {
            Status = LotRequestStatus.Bet;
        }

        public int LotId { get; set; }

        public int DispatcherId { get; set; }

        public LotRequestStatus Status { get; set; }
    }
}