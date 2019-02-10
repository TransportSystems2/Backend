using Common.Models;
using DotNetDistance;
using System;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.Application.Models.Booking
{
    public class BookingRouteAM : BaseAM
    {
        public string Title { get; set; }

        public AddressAM RootAddress { get; set; }

        public TimeBelt RootAddressTimeBelt { get; set; }

        public Distance TotalDistance { get; set; }

        public Distance FeedDistance { get; set; }

        public TimeSpan AvgDeliveryTime { get; set; }

        public BillAM Bill { get; set;  }
    }
}