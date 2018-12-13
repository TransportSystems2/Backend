using TransportSystems.Backend.Core.Domain.Core.Catalogs;

namespace TransportSystems.Backend.Application.Models.Catalogs
{
    public class CatalogItemAM : DomainAM
    {
        public CatalogItemKind Kind { get; set; }

        public string Name { get; set;  }

        public int Value { get; set; }
    }
}