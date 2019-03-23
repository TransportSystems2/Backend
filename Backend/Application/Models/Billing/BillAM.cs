using System.Collections.Generic;

namespace TransportSystems.Backend.Application.Models.Billing
{
    public class BillAM : BaseAM
    {
        public BillAM()
        {
            Items = new List<BillItemAM>();
        }

        public BillInfoAM Info { get; set; }

        public BasketAM Basket { get; set; }

        public List<BillItemAM> Items { get; }

        public decimal TotalCost { get; set; }
    }
}