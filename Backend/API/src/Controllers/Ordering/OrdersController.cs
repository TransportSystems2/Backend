using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Booking;

namespace TransportSystems.API.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        public OrdersController(IApplicationOrderService service)
        {
            Service = service;
        }

        protected IApplicationOrderService Service { get; }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]BookingAM booking)
        {
            await Service.CreateOrder(booking);

            return Ok();
        }
    }
}