using TransportSystems.Backend.Core.Domain.Core.Ordering;

namespace TransportSystems.Backend.Application.Models.Ordering
{
    public class OrderGroupAM : BaseAM
    {
        public string Title { get; set; }

        public OrderStatus Status { get; set; }

        public int Count { get; set; }
    }
}