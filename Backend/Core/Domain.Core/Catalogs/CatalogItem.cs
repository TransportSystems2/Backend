using System;
namespace TransportSystems.Backend.Core.Domain.Core.Catalogs
{
    public class CatalogItem : BaseEntity
    {
        public int CatalogId { get; set; }

        public CatalogItemKind Kind { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }
    }
}
