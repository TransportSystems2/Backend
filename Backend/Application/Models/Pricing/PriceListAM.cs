using System.Collections.Generic;

namespace TransportSystems.Backend.Application.Models.Pricing
{
    public class PricelistAM : BaseAM
    {
        public PricelistAM()
        {
            Items = new List<PriceAM>();
        }

        public List<PriceAM> Items { get; }
    }
}