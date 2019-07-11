using TransportSystems.Backend.Application.Models.Catalogs;

namespace TransportSystems.Backend.Application.Models.Transport
{
    public class VehicleAM : DomainAM
    {
        public CatalogItemAM BrandCatalogItem { get; set; }

        public CatalogItemAM CapacityCatalogItem { get; set; }

        public CatalogItemAM KindCatalogItem { get; set; }

        public string RegistrationNumber { get; set; }
    }
}