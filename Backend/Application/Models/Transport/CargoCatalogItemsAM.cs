using System.Collections.Generic;
using TransportSystems.Backend.Application.Models.Catalogs;

namespace TransportSystems.Backend.Application.Models.Transport
{
    public class CargoCatalogItemsAM : BaseAM
    {
        public CargoCatalogItemsAM()
        {
            Brands = new List<CatalogItemAM>();
            Weights = new List<CatalogItemAM>();
            Kinds = new List<CatalogItemAM>();
        }

        public List<CatalogItemAM> Brands { get; set; }

        public List<CatalogItemAM> Weights { get; set; }

        public List<CatalogItemAM> Kinds { get; set; }
    }
}