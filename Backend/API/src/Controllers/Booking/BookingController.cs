using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Models.Booking;

namespace TransportSystems.Backend.API.Controllers.Booking
{
    [Route("api/[controller]")]
    public class BookingController : Controller
    {
        public BookingController(IApplicationBookingService service)
        {
            Service = service;
        }

        protected IApplicationBookingService Service { get; }

        [HttpPost]
        public Task<BookingResponseAM> Calculate([FromBody]BookingRequestAM request)
        {
            return Service.CalculateBooking(request); 
        }
    }
}