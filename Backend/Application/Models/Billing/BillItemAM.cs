namespace TransportSystems.Backend.Application.Models.Billing
{
    public class BillItemAM : BaseAM
    {
        public string Key { get; set; }

        public int Value { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }
    }
}