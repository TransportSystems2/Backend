using System.Collections.Generic;

namespace TransportSystems.Backend.Application.Models.Booking
{
    public class BookingResponseAM : BaseAM
    {
        public BookingResponseAM()
        {
            Routes = new List<BookingRouteAM>();
        }

        public List<BookingRouteAM> Routes { get; }
    }
}