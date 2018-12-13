namespace TransportSystems.Backend.Application.Models.Transport
{
    public class CargoAM : BaseAM
    {
        public int WeightCatalogItemId { get; set; }

        public int KindCatalogItemId { get; set; }

        public int BrandCatalogItemId { get; set; }

        public string RegistrationNumber { get; set; }

        public string Comment { get; set; }
    }
}