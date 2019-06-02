using DotNetDistance;
using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Infrastructure.Business.Billing;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Billing
{
    public class BasketServiceTestSuite
    {
        public BasketServiceTestSuite()
        {
            BillRepositoryMock = new Mock<IBasketRepository>();

            BillService = new BasketService(BillRepositoryMock.Object);
        }
        public IBasketService BillService { get; }

        public Mock<IBasketRepository> BillRepositoryMock { get; }
    }

    public class BasketServiceTests
    {
        public BasketServiceTests()
        {
            Suite = new BasketServiceTestSuite();
        }

        protected BasketServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreateBasket()
        {
            var basket = new Basket
            {
                Distance = Distance.FromKilometers(762),
                LoadingValue = 1,
                LockedSteeringValue = 0,
                LockedWheelsValue = 3,
                OverturnedValue = 1,
                DitchValue = 0
            };

            var result = await Suite.BillService.Create(
                basket.Distance,
                basket.LoadingValue,
                basket.LockedSteeringValue,
                basket.LockedWheelsValue,
                basket.OverturnedValue,
                basket.DitchValue);

            Suite.BillRepositoryMock
                .Verify(m => m.Add(It.Is<Basket>(
                    newBill => newBill.Distance.Equals(basket.Distance)
                    && newBill.LoadingValue.Equals(basket.LoadingValue)
                    && newBill.LockedSteeringValue.Equals(basket.LockedSteeringValue)
                    && newBill.LockedWheelsValue.Equals(basket.LockedWheelsValue)
                    && newBill.OverturnedValue.Equals(basket.OverturnedValue)
                    && newBill.DitchValue.Equals(basket.DitchValue))));
            Suite.BillRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(basket.Distance, result.Distance);
            Assert.Equal(basket.LoadingValue, result.LoadingValue);
            Assert.Equal(basket.LockedSteeringValue, result.LockedSteeringValue);
            Assert.Equal(basket.LockedWheelsValue, result.LockedWheelsValue);
            Assert.Equal(basket.OverturnedValue, result.OverturnedValue);
            Assert.Equal(basket.DitchValue, result.DitchValue);
        }
    }
}