namespace TransportSystems.Backend.Core.Domain.Core.Organization
{
    public class City : BaseEntity
    {
        public string Domain { get; set; }

        public int AddressId { get; set; }

        public int PricelistId { get; set; }
    }
}