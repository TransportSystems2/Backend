using System.Collections.Generic;
using TransportSystems.Backend.Application.Models.Catalogs;

namespace TransportSystems.Backend.Application.Models.Transport
{
    public class VehicleCatalogItemsAM : BaseAM
    {
        public VehicleCatalogItemsAM()
        {
            Brands = new List<CatalogItemAM>();
            Kinds = new List<CatalogItemAM>();
            Capacities = new List<CatalogItemAM>();
        }

        public List<CatalogItemAM> Brands { get; }

        public List<CatalogItemAM> Kinds { get; }

        public List<CatalogItemAM> Capacities { get; }
    }
}