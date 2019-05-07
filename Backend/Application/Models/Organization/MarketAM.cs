using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Pricing;

namespace TransportSystems.Backend.Application.Models.Organization
{
    public class MarketAM : BaseAM
    {
        public AddressAM Address { get; set; }
        
        public PricelistAM Pricelist { get; set; }
    }
}