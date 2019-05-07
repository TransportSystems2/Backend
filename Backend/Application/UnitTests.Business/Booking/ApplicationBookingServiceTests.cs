using Common.Models;
using Common.Models.Units;
using DotNetDistance;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Booking;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Booking;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Prognosing;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Interfaces.Users;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Routing;
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
            PrognosisServiceMock = new Mock<IApplicationPrognosisService>();
            MarketServiceMock = new Mock<IApplicationMarketService>();
            UserServiceMock = new Mock<IApplicationUserService>();

            BookingService = new ApplicationBookingService(
                AddressServiceMock.Object,
                BillServiceMock.Object,
                RouteServiceMock.Object,
                PrognosisServiceMock.Object,
                MarketServiceMock.Object,
                UserServiceMock.Object);
        }

        public IApplicationBookingService BookingService { get; }

        public Mock<IApplicationAddressService> AddressServiceMock { get; }

        public Mock<IApplicationBillService> BillServiceMock { get; }

        public Mock<IApplicationRouteService> RouteServiceMock { get; }

        public Mock<IApplicationPrognosisService> PrognosisServiceMock { get; }

        public Mock<IApplicationMarketService> MarketServiceMock { get; }

        public Mock<IApplicationUserService> UserServiceMock { get; }
    }

    public class ApplicationBookingServiceTests : BaseServiceTests<ApplicationBookingServiceTestSuite>
    {
        [Fact]
        public async Task CalculateBookingRoute()
        {
            var commonId = 1;

            var marketAddress = new AddressAM
            {
                Locality = "Ярославль",
                Latitude = 11.1111,
                Longitude = 22.2222
            };

            var rootAddressTimeBelt = new TimeBelt();

            var billInfo = new BillInfoAM
            {
                PriceId = commonId++,
                CommissionPercentage = 10,
                DegreeOfDifficulty = 1
            };

            var basket = new BasketAM
            {
                Distance = Distance.FromKilometers(0),
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
                    new RouteLegAM { Kind = RouteLegKind.Feed, Distance = Distance.FromKilometers(30), Duration = TimeSpan.FromMinutes(10) },
                    new RouteLegAM { Kind = RouteLegKind.Transportation, Distance = Distance.FromKilometers(100), Duration = TimeSpan.FromMinutes(600) },
                    new RouteLegAM { Kind = RouteLegKind.WayBack, Distance = Distance.FromKilometers(103), Duration = TimeSpan.FromMinutes(650) }
                }
            };

            var waypoints = new WaypointsAM();

            var domainMarket = new Market
            {
                Id = commonId++,
                PricelistId = commonId++,
                AddressId = commonId++
            };

            var feedDistance = route.Legs[0].Distance;
            var avgDeliveryTime = TimeSpan.FromMinutes(30);
            var totalDistance = route.Legs.Select(l => l.Distance).Sum();

            var bill = new BillAM { TotalCost = 100 };
            var title = $"{marketAddress.Locality} - {bill.TotalCost}₽";

            Suite.AddressServiceMock
                .Setup(m => m.GetAddress(domainMarket.AddressId))
                .ReturnsAsync(marketAddress);
            Suite.RouteServiceMock
                .Setup(m => m.GetRoute(marketAddress, waypoints))
                .ReturnsAsync(route);
            Suite.PrognosisServiceMock
                .Setup(m => m.GetAvgDeliveryTime(route, cargo, basket))
                .ReturnsAsync(avgDeliveryTime);
            Suite.RouteServiceMock
                .Setup(m => m.GetRootAddress(route))
                .Returns(marketAddress);
            Suite.RouteServiceMock
                .Setup(m => m.GetFeedDistance(route))
                .Returns(feedDistance);
            Suite.RouteServiceMock
                .Setup(m => m.GetTotalDistance(route))
                .Returns(totalDistance);
            Suite.AddressServiceMock
                .Setup(m => m.GetTimeBeltByAddress(marketAddress))
                .ReturnsAsync(rootAddressTimeBelt);

            Suite.BillServiceMock
                .Setup(m => m.GetBillInfo(domainMarket.PricelistId, cargo.WeightCatalogItemId))
                .ReturnsAsync(billInfo);
            Suite.BillServiceMock
                .Setup(m => m.CalculateBill(
                    billInfo,
                    It.Is<BasketAM>(b => (b != basket) && b.Distance.Equals(totalDistance))))
                .ReturnsAsync(bill);

            var result = await Suite.BookingService.CalculateBookingRoute(domainMarket, waypoints, cargo, basket);

            Assert.Equal(domainMarket.Id, result.MarketId);
            Assert.Equal(rootAddressTimeBelt, result.MarketTimeBelt);
            Assert.Equal(feedDistance, result.FeedDistance);
            Assert.Equal(avgDeliveryTime, result.AvgDeliveryTime);
            Assert.Equal(totalDistance, result.TotalDistance);
            Assert.Equal(bill, result.Bill);
            Assert.Equal(title, result.Title);
        }
    }
}