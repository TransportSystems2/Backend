using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.API.Controllers.Extensions;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Ordering;
using TransportSystems.Backend.Core.Domain.Core.Users;

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
            var userIdentityId = User.GetIdentityId();

            var dispatcherId = 0;
            if (userIdentityId != null)
            {
                var dispatcher = await UserService.GetDomainDispatcherByIdentityUser(userIdentityId.Value);
                if (dispatcher != null)
                {
                    dispatcherId = dispatcher.Id;
                }
            }

            await Service.CreateOrder(booking, dispatcherId);

            return Ok();
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
            var userIdentityId = User.GetIdentityId();
            var dispatcher = await UserService.GetDomainDispatcherByIdentityUser(userIdentityId.Value);

            await Service.Accept(orderId, dispatcher.Id);

            return Ok();
        }

        [HttpGet("readyToTrade")]
        public async Task<IActionResult> ReadyToTrade(int orderId)
        {
            var userIdentityId = User.GetIdentityId();
            var dispatcher = await UserService.GetDomainDispatcherByIdentityUser(userIdentityId.Value);

            await Service.ReadyToTrade(orderId, dispatcher.Id);

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
        public async Task<IActionResult> AssignToDriver(int orderId, int driverId)
        {
            var userIdentityId = User.GetIdentityId();
            var subDispatcher = await UserService.GetDomainDispatcherByIdentityUser(userIdentityId.Value);

            await Service.AssignToDriver(orderId, subDispatcher.Id, driverId);

            return Ok();
        }
    }
}