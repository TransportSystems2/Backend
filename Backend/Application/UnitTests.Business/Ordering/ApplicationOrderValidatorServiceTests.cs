using DotNetDistance;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Ordering;
using TransportSystems.Backend.Application.Interfaces.Ordering;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Ordering
{
    public class ApplicationOrderValidatorServiceTestSuite : BaseTestsSuite
    {
        public ApplicationOrderValidatorServiceTestSuite()
        {
            RouteServiceMock = new Mock<IApplicationRouteService>();

            Service = new ApplicationOrderValidatorService(RouteServiceMock.Object);
        }

        public IApplicationOrderValidatorService Service { get; }

        public Mock<IApplicationRouteService> RouteServiceMock { get; }
    }

    public class ApplicationOrderValidatorServiceTests : BaseServiceTests<ApplicationOrderValidatorServiceTestSuite>
    {
        [Fact]
        public async Task ValidateWhenBookingTotalCostDoesNotEqualsOrderTotalCost()
        {
            var booking = new BookingAM
            {
                Bill = new BillAM
                {
                    TotalCost = 10000
                }
            };

            var orderRoute = new RouteAM();

            var orderBill = new BillAM
            {
                TotalCost = 9000
            };

            await Assert.ThrowsAsync<ValidationException>(() => Suite.Service.Validate(booking, orderRoute, orderBill));
        }

        [Fact]
        public async Task ValidateWhereBookingDistanceDoesNotEqualOrderDistance()
        {
            var booking = new BookingAM
            {
                Bill = new BillAM
                {
                    TotalCost = 10000,
                    Basket = new BasketAM
                    {
                        Distance = Distance.FromKilometers(70)
                    }
                }
            };

            var orderRoute = new RouteAM();

            var orderBill = new BillAM
            {
                TotalCost = 10000
            };

            Suite.RouteServiceMock
                .Setup(m => m.GetTotalDistance(orderRoute))
                .Returns(Distance.FromKilometers(80));

            await Assert.ThrowsAsync<ValidationException>(() => Suite.Service.Validate(booking, orderRoute, orderBill));
        }
    }
}