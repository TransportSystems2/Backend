using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Business.Prognosing;
using TransportSystems.Backend.Application.Interfaces.Prognosing;
using TransportSystems.Backend.Application.Interfaces.Routing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Models.Routing;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.UnitTests.Business.Suite;
using TransportSystems.Backend.Core.Domain.Core.Routing;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business.Prognosing
{
    public class ApplicationPrognosisServiceTestsSuite : MappingTestsSuite
    {
        public ApplicationPrognosisServiceTestsSuite()
        {
            RouteServiceMock = new Mock<IApplicationRouteService>();

            PrognosisService = new ApplicationPrognosisService(
                RouteServiceMock.Object);
        }

        public IApplicationPrognosisService PrognosisService { get; }

        public Mock<IApplicationRouteService> RouteServiceMock { get; }
    }

    public class ApplicationPrognosisServiceTests : BaseServiceTests<ApplicationPrognosisServiceTestsSuite>
    {
        [Fact]
        public async Task GetAvgDeliveryTime()
        {
            var route = new RouteAM
            {
                Legs =
                {
                    new RouteLegAM { Duration = TimeSpan.FromMinutes(17), Kind = RouteLegKind.Feed }
                }
            };

            var cargo = new CargoAM();
            var basket = new BasketAM();
            var feedDurationTimeSpan = route.Legs[0].Duration;

            Suite.RouteServiceMock
                .Setup(m => m.GetFeedDuration(route))
                .ReturnsAsync(feedDurationTimeSpan);

            var result = await Suite.PrognosisService.GetAvgDeliveryTime(route, cargo, basket);

            var avgDeliveryTime = new TimeSpan();
            avgDeliveryTime = avgDeliveryTime.Add(feedDurationTimeSpan);
            avgDeliveryTime = avgDeliveryTime.Add(ApplicationPrognosisService.DefaultAvgPreparationDeiverTime);
            avgDeliveryTime = avgDeliveryTime.Add(ApplicationPrognosisService.DefaultAvgTradingTime);

            Assert.Equal(avgDeliveryTime, result);
        }
    }
}