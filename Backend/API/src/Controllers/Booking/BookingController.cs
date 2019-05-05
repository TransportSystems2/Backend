using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        [HttpPost("calculate")]
        public Task<BookingResponseAM> Calculate([FromBody]BookingRequestAM request)
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Service.CalculateBooking(int.Parse(identityUserId), request); 
        }
    }
}