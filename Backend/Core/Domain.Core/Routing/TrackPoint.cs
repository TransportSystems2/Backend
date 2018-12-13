namespace TransportSystems.Backend.Core.Domain.Core.Routing
{
    public class TrackPoint : BaseEntity
    {
        public int TrackId { get; set; }

        public int Timestamp { get; set; }

        public byte Speed { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}