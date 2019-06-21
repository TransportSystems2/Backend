using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using TransportSystems.API.Controllers;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Core.Domain.Core.Users;
using Xunit;

namespace TransportSystems.Backend.API.Tests.Controllers.Ordering
{
    public class OrdersControllerTestSuite
    {
        public OrdersControllerTestSuite()
        {
            ServiceMock = new Mock<IApplicationOrderService>();
            UserServiceMock = new Mock<IApplicationUserService>();

            Controller = new OrdersController(ServiceMock.Object, UserServiceMock.Object);
        }

        public OrdersController Controller { get; set; }

        public Mock<IApplicationOrderService> ServiceMock { get; }

        public Mock<IApplicationUserService> UserServiceMock { get; }
    }

    public class OrdersControllerTests
    {
        public OrdersControllerTests()
        {
            Suite = new OrdersControllerTestSuite();
        }

        protected OrdersControllerTestSuite Suite { get; }

        [Fact]
        public async Task Create()
        {
            var commonId = 1;
            var booking = new BookingAM();
            var dispatcher = new Dispatcher
            {
                Id = commonId++,
            };

            Suite.Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, "username"),
                            new Claim("sub", dispatcher.Id.ToString())
                        },
                        IdentityUser.DispatcherRoleName))
                }
            };

            Suite.UserServiceMock
                .Setup(m => m.GetDomainDispatcher(dispatcher.Id))
                .ReturnsAsync(dispatcher);

            await Suite.Controller.Create(booking);

            Suite.ServiceMock
                .Verify(m => m.CreateOrder(booking, dispatcher.Id));
        }

        [Fact]
        public async Task GetDetailInfo()
        {
            var commonId = 1;

            var orderId = commonId++;
            
            await Suite.Controller.GetDetailInfo(orderId);

            Suite.ServiceMock
                .Verify(m => m.GetDetailInfo(orderId));
        }
    }
}