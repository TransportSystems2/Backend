using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.API.Controllers.Extensions;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Ordering;

namespace TransportSystems.API.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        public OrdersController(
            IApplicationOrderService service,
            IApplicationUserService userService)
        {
            Service = service;
            UserService = userService;
        }

        protected IApplicationOrderService Service { get; }

        protected IApplicationUserService UserService { get; }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]BookingAM booking)
        {
            var userId = User.GetIdentityId();
            await Service.CreateOrder(booking, userId ?? 0);

            return Ok();
        }

        [HttpGet("detail_info")]
        public Task<DetailOrderInfoAM> GetDetailInfo(int orderId)
        {
            return Service.GetDetailInfo(orderId);
        }

        [HttpGet("groups")]
        public Task<ICollection<OrderGroupAM>> GetGroups(OrderStatus[] statuses)
        {
            return Service.GetGroupsByStatuses(statuses);
        }

        [HttpGet("group")]
        public Task<ICollection<OrderInfoAM>> GetGroup(OrderStatus status)
        {
            return Service.GetGroupByStatus(status);
        }

        [HttpGet("accept")]
        public async Task<IActionResult> Accept(int orderId)
        {
            var userId = User.GetIdentityId();
            await Service.Accept(orderId, userId.Value);

            return Ok();
        }

        [HttpGet("readyToTrade")]
        public async Task<IActionResult> ReadyToTrade(int orderId)
        {
            var userId = User.GetIdentityId();
            await Service.ReadyToTrade(orderId, userId.Value);

            return Ok();
        }

        [HttpGet("trade")]
        public async Task<IActionResult> Trade(int orderId)
        {
            await Service.Trade(orderId);

            return Ok();
        }

        [HttpGet("assignToSubDispatcher")]
        public async Task<IActionResult> AssignToSubDispatcher(int orderId, int subDispatcherId)
        {
            await Service.AssignToSubDispatcher(orderId, subDispatcherId);

            return Ok();
        }

        [HttpGet("assignToDriver")]
        public async Task<IActionResult> AssignToDriver(int orderId, int driverId, int vehicleId)
        {
            var userId = User.GetIdentityId();
            await Service.AssignToDriver(orderId, userId.Value, driverId, vehicleId);

            return Ok();
        }
    }
}