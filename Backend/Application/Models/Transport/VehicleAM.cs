namespace TransportSystems.Backend.Application.Models.Transport
{
    public class VehicleAM : BaseAM
    {
        public int BrandCatalogItemId { get; set; }

        public int CapacityCatalogItemId { get; set; }

        public int KindCatalogItemId { get; set; }

        public string RegistrationNumber { get; set; }
    }
}