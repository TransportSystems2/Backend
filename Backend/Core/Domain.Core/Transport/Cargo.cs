namespace TransportSystems.Backend.Core.Domain.Core.Transport
{
    public class Cargo : BaseEntity
    {
        public int WeightCatalogItemId { get; set; }

        public int KindCatalogItemId { get; set; }

        public int BrandCatalogItemId { get; set; }

        public string RegistrationNumber { get; set; }

        public string Comment { get; set; }
    }
}