using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Ordering;

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

        [HttpGet("groups")]
        public Task<ICollection<OrderGroupAM>> GetGroups(OrderStatus[] statuses)
        {
            return Service.GetOrderGroupsByStatuses(statuses);
        }

        [HttpGet("orders_info_by_status")]
        public Task<ICollection<OrderInfoAM>> GetOrdersInfoByStatus(OrderStatus status)
        {
            return Service.GetOrdersByStatus(status);
        }
    }
}