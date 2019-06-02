namespace TransportSystems.Backend.Core.Domain.Core.Organization
{
    public class Market : BaseEntity
    {
        public int CompanyId { get; set; }

        public int PricelistId { get; set; }

        public int AddressId { get; set; }
    }
}