using Common.Models.Geolocation;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Geo;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using TransportSystems.Backend.Application.Business.Booking;
using TransportSystems.Backend.Application.Business.Geo;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Booking;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Booking
{
    public class ApplicationBookingServiceTestSuite : BaseTestsSuite
    {
        public ApplicationBookingServiceTestSuite()
        {
            AddressServiceMock = new Mock<IApplicationAddressService>();
            BillServiceMock = new Mock<IApplicationBillService>();
            RouteServiceMock = new Mock<IApplicationRouteService>();

            BookingService = new ApplicationBookingService(
                AddressServiceMock.Object,
                BillServiceMock.Object,
                RouteServiceMock.Object);
        }

        public IApplicationBookingService BookingService { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }

        public Mock<IApplicationBillService> BillServiceMock { get; }

        public Mock<IApplicationRouteService> RouteServiceMock { get; }
    }

    public class ApplicationBookingServiceTests : BaseServiceTests<ApplicationBookingServiceTestSuite>
    {
        [Fact]
        public async Task CalculateBookingRoute()
        {
            var commonId = 1;

            var rootAddress = new AddressAM
            {
                Locality = "Ярославль",
                Latitude = 11.1111,
                Longitude = 22.2222
            };

            var billInfo = new BillInfoAM
            {
                PriceId = commonId++,
                CommissionPercentage = 10,
                DegreeOfDifficulty = 1
            };

            var basket = new BasketAM
            {
                KmValue = 0,
                LoadingValue = 1,
                LockedSteeringValue = 0,
                LockedWheelsValue = 3,
                OverturnedValue = 0,
                DitchValue = 1
            };

            var cargo = new CargoAM
            {
                BrandCatalogItemId = commonId++,
                KindCatalogItemId = commonId++,
                RegistrationNumber = "e111ey777",
                WeightCatalogItemId = commonId++
            };

            var route = new RouteAM
            {
                Legs =
                {
                    new RouteLegAM { Kind = RouteLegKind.Feed, Distance = 3000, Duration = 1200 },
                    new RouteLegAM { Kind = RouteLegKind.Transportation, Distance = 100000, Duration = 7200 },
                    new RouteLegAM { Kind = RouteLegKind.WayBack, Distance = 103000, Duration = 8400 }
                }
            };

            var feedDistance = route.Legs[0].Distance;
            var feedDuration = route.Legs[0].Duration;
            var totalDistance = route.Legs.Select(l => l.Distance).Sum();

            var bill = new BillAM {  TotalCost = 100 };
            var title = $"{rootAddress.Locality} - {bill.TotalCost}₽";

            Suite.RouteServiceMock
                .Setup(m => m.GetRootAddress(route))
                .Returns(rootAddress);
            Suite.RouteServiceMock
                .Setup(m => m.GetFeedDistance(route))
                .Returns(feedDistance);
            Suite.RouteServiceMock
                .Setup(m => m.GetFeedDuration(route))
                .Returns(feedDuration);
            Suite.RouteServiceMock
                .Setup(m => m.GetTotalDistance(route))
                .Returns(totalDistance);

            Suite.BillServiceMock
                .Setup(m => m.GetBillInfo(
                    It.Is<Coordinate>(c => c.Latitude.Equals(rootAddress.Latitude) && c.Longitude.Equals(rootAddress.Longitude)),
                    cargo.WeightCatalogItemId))
                .ReturnsAsync(billInfo);
            Suite.BillServiceMock
                .Setup(m => m.CalculateBill(
                    billInfo,
                    It.Is<BasketAM>(b => (b != basket) && b.KmValue.Equals(totalDistance))))
                .ReturnsAsync(bill);

            var result = await Suite.BookingService.CalculateBookingRoute(route, cargo, basket);

            Assert.Equal(rootAddress, result.RootAddress);
            Assert.Equal(feedDistance, result.FeedDistance);
            Assert.Equal(feedDuration, result.FeedDuration);
            Assert.Equal(totalDistance, result.TotalDistance);
            Assert.Equal(bill, result.Bill);
            Assert.Equal(title, result.Title);
        }
    }
}