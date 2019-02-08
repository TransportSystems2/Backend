namespace TransportSystems.Backend.Core.Domain.Core.Billing
{
    public class BillItem : BaseEntity
    {
        public const string MetersBillKey = "MetersBill";
        public const string LoadingBillKey = "LoadingBill";
        public const string LockedSteeringKey = "LockedSteeringBill";
        public const string LockedWheelsKey = "LockedWheelsBill";
        public const string OverturnedKey = "OverturnedBill";
        public const string DitchKey = "DitchBill";

        public int BillId { get; set; }

        public string Key { get; set; }

        public int Value { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }
    }
}