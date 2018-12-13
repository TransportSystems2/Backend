using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.API.Controllers.Booking;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Models.Booking;
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

            Suite.ApplicationServiceMock
                .Setup(m => m.CalculateBooking(bookingRequest))
                .ReturnsAsync(bookingResponse);

            var result = await Suite.Controller.Calculate(bookingRequest);

            Suite.ApplicationServiceMock
                .Verify(m => m.CalculateBooking(bookingRequest));

            Assert.Equal(bookingResponse, result);
        }
    }
}
