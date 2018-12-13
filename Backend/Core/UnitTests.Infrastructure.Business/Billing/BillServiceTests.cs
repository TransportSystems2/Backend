using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Infrastructure.Business.Billing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Billing
{
    public class BillServiceTestSuite
    {
        public BillServiceTestSuite()
        {
            BillRepositoryMock = new Mock<IBillRepository>();
            PriceServiceMock = new Mock<IPriceService>();
            BasketServiceMock = new Mock<IBasketService>();

            BillService = new BillService(BillRepositoryMock.Object, PriceServiceMock.Object, BasketServiceMock.Object);
        }
        public IBillService BillService { get; }

        public Mock<IBillRepository> BillRepositoryMock { get; }

        public Mock<IPriceService> PriceServiceMock { get; }

        public Mock<IBasketService> BasketServiceMock { get; }
    }

    public class BillServiceTests
    {
        public BillServiceTests()
        {
            Suite = new BillServiceTestSuite();
        }

        protected BillServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreateBill()
        {
            var commonId = 1;
            var bill = new Bill
            {
                PriceId = commonId++,
                BasketId = commonId++,
                CommissionPercentage = 10,
                DegreeOfDifficulty = 1,
            };

            Suite.PriceServiceMock
                .Setup(m => m.IsExist(bill.PriceId))
                .ReturnsAsync(true);
            Suite.BasketServiceMock
                .Setup(m => m.IsExist(bill.BasketId))
                .ReturnsAsync(true);

            var result = await Suite.BillService.Create(
                bill.PriceId,
                bill.BasketId,
                bill.CommissionPercentage,
                bill.DegreeOfDifficulty);

            Suite.BillRepositoryMock
                .Verify(m => m.Add(It.Is<Bill>(
                    newBill => newBill.PriceId.Equals(bill.PriceId)
                    && newBill.BasketId.Equals(bill.BasketId)
                    && newBill.CommissionPercentage.Equals(bill.CommissionPercentage)
                    && newBill.DegreeOfDifficulty.Equals(bill.DegreeOfDifficulty))));
            Suite.BillRepositoryMock
                .Verify(m => m.Save());

            Assert.Equal(bill.PriceId, result.PriceId);
            Assert.Equal(bill.CommissionPercentage, result.CommissionPercentage);
            Assert.Equal(bill.DegreeOfDifficulty, result.DegreeOfDifficulty);
        }

        [Fact]
        public async Task CreateBillWherePriceDoesNotExist()
        {
            var priceId = 1;

            Suite.PriceServiceMock
                .Setup(m => m.IsExist(priceId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Price", () => Suite.BillService.Create(priceId, 0, 0, 0));
        }

        [Fact]
        public async Task CreateBillWhereBasketDoesNotExist()
        {
            var priceId = 1;
            var basketId = 2;

            Suite.PriceServiceMock
                .Setup(m => m.IsExist(priceId))
                .ReturnsAsync(true);
            Suite.BasketServiceMock
                .Setup(m => m.IsExist(basketId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Basket", () => Suite.BillService.Create(priceId, basketId, 0, 0));
        }

        [Fact]
        public async Task CreateBillWhereCommissionPercentageIsGreateThanAllowedRange()
        {
            var priceId = 1;
            var basketId = 2;
            var commissionPercentage = (byte)101;

            Suite.PriceServiceMock
                .Setup(m => m.IsExist(priceId))
                .ReturnsAsync(true);
            Suite.BasketServiceMock
                .Setup(m => m.IsExist(basketId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "CommissionPercentage",
                () => Suite.BillService.Create(priceId, basketId, commissionPercentage, 0));
        }

        [Fact]
        public async Task CreateBillWhereDegreeOfDifficultyIsGreaterThanAllowedRange()
        {
            var priceId = 1;
            var basketId = 2;
            var commissionPercentage = (byte)10;
            var degreeOfDifficulty = BillService.MaxDegreeOfDifficulty + 1;

            Suite.PriceServiceMock
                .Setup(m => m.IsExist(priceId))
                .ReturnsAsync(true);
            Suite.BasketServiceMock
                .Setup(m => m.IsExist(basketId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "DegreeOfDifficulty",
                () => Suite.BillService.Create(priceId, basketId, commissionPercentage, degreeOfDifficulty));
        }

        [Fact]
        public async Task CreateBillWhereDegreeOfDifficultyIsLessThanAllowedRange()
        {
            var priceId = 1;
            var basketId = 2;
            var commissionPercentage = (byte)10;
            var degreeOfDifficulty = BillService.MinDegreeOfDifficulty - 1;

            Suite.PriceServiceMock
                .Setup(m => m.IsExist(priceId))
                .ReturnsAsync(true);
            Suite.BasketServiceMock
                .Setup(m => m.IsExist(basketId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                "DegreeOfDifficulty",
                () => Suite.BillService.Create(priceId, basketId, commissionPercentage, degreeOfDifficulty));
        }

        [Fact]
        public async Task SetTotalCost()
        {
            var bill = new Bill
            {
                Id = 1,
                TotalCost = 0m
            };

            var totalCost = 123m;

            Suite.BillRepositoryMock
                .Setup(m => m.Get(bill.Id))
                .ReturnsAsync(bill);

            await Suite.BillService.SetTotalCost(bill.Id, totalCost);

            Suite.BillRepositoryMock
                .Verify(m => m.Update(It.Is<Bill>(
                    b => b.TotalCost.Equals(totalCost))));
            Suite.BillRepositoryMock
                .Verify(m => m.Save());
        }
    }
}