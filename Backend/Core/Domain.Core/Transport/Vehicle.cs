namespace TransportSystems.Backend.Core.Domain.Core.Transport
{
    public class Vehicle : BaseEntity
    {
        public int CompanyId { get; set; }

        public string RegistrationNumber { get; set; }

        public int BrandCatalogItemId { get; set; }

        public int CapacityCatalogItemId { get; set; }

        public int KindCatalogItemId { get; set; }
    }
}