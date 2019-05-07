using Common.Models;
using DotNetDistance;
using System;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Organization;

namespace TransportSystems.Backend.Application.Models.Booking
{
    public class BookingRouteAM : BaseAM
    {
        public string Title { get; set; }

        public int MarketId { get; set; }

        public TimeBelt MarketTimeBelt { get; set; }

        public Distance TotalDistance { get; set; }

        public Distance FeedDistance { get; set; }

        public TimeSpan AvgDeliveryTime { get; set; }

        public BillAM Bill { get; set;  }
    }
}