namespace TransportSystems.Backend.Core.Domain.Core.Organization
{
    public class Garage : BaseEntity
    {
        public int AddressId { get; set; }

        public int PricelistId { get; set; }

        public int CompanyId { get; set; }

        public bool IsPublic { get; set; }
    }
}