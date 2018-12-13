using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Models.Booking
{
    public class BookingRouteAM : BaseAM
    {
        public string Title { get; set; }

        public AddressAM RootAddress { get; set; }

        public int TotalDistance { get; set; }

        public int FeedDistance { get; set; }

        public int FeedDuration { get; set; }

        public BillAM Bill { get; set;  }
    }
}