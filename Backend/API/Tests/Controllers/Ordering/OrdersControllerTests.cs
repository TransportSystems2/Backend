using Moq;
using System.Threading.Tasks;
using TransportSystems.API.Controllers;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Booking;
using Xunit;

namespace TransportSystems.Backend.API.Tests.Controllers.Ordering
{
    public class OrdersControllerTestSuite
    {
        public OrdersControllerTestSuite()
        {
            ApplicationServiceMock = new Mock<IApplicationOrderService>();
            Controller = new OrdersController(ApplicationServiceMock.Object);
        }

        public OrdersController Controller { get; set; }

        public Mock<IApplicationOrderService> ApplicationServiceMock { get; }
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
            var booking = new BookingAM();

            await Suite.Controller.Create(booking);

            Suite.ApplicationServiceMock
                .Verify(m => m.CreateOrder(booking));
        }
    }
}