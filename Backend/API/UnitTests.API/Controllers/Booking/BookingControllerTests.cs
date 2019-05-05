using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using TransportSystems.Backend.API.Controllers.Booking;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Core.Domain.Core.Users;
using Xunit;

namespace TransportSystems.Backend.API.UnitTests.API.Controllers.Booking
{
    public class BookingControllerTestSuite
    {
        public BookingControllerTestSuite()
        {
            ApplicationServiceMock = new Mock<IApplicationBookingService>();
            Controller = new BookingController(ApplicationServiceMock.Object);
        }

        public BookingController Controller { get; set; }

        public Mock<IApplicationBookingService> ApplicationServiceMock { get; }
    }

    public class BookingControllerTests
    {
        public BookingControllerTests()
        {
            Suite = new BookingControllerTestSuite();
        }

        protected BookingControllerTestSuite Suite { get; }

        [Fact]
        public async Task Calculate()
        {
            var bookingRequest = new BookingRequestAM();
            var bookingResponse = new BookingResponseAM();

            var identityUserId = 1;

            Suite.Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, "username"),
                            new Claim(ClaimTypes.NameIdentifier, identityUserId.ToString())
                        },
                        UserRole.DispatcherRoleName))
                }
            };

            Suite.ApplicationServiceMock
                .Setup(m => m.CalculateBooking(identityUserId, bookingRequest))
                .ReturnsAsync(bookingResponse);

            var result = await Suite.Controller.Calculate(bookingRequest);

            Suite.ApplicationServiceMock
                .Verify(m => m.CalculateBooking(identityUserId, bookingRequest));

            Assert.Equal(bookingResponse, result);
        }
    }
}
