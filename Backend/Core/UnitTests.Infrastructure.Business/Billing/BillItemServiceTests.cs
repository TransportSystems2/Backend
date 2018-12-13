using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Infrastructure.Business.Billing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Billing
{
    public class BillItemServiceTestSuite
    {
        public BillItemServiceTestSuite()
        {
            BillItemRepositoryMock = new Mock<IBillItemRepository>();
            BillServiceMock = new Mock<IBillService>();

            BillItemService = new BillItemService(BillItemRepositoryMock.Object, BillServiceMock.Object);
        }
        public IBillItemService BillItemService { get; }

        public Mock<IBillItemRepository> BillItemRepositoryMock { get; }

        public Mock<IBillService> BillServiceMock { get; }
    }

    public class BillItemServiceTests
    {
        public BillItemServiceTests()
        {
            Suite = new BillItemServiceTestSuite();
        }

        protected BillItemServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreateBillItem()
        {
            var billId = 1;
            var key = "SomeKey";
            var value = 123;
            var price = 123.45m;

            var cost = value * price;
            var totalCost = 99999m;

            Suite.BillServiceMock
                .Setup(m => m.IsExist(billId))
                .ReturnsAsync(true);

            Suite.BillItemRepositoryMock
                .Setup(m => m.GetTotalCost(billId))
                .ReturnsAsync(totalCost);

            var result = await Suite.BillItemService.Create(billId, key, value, price, cost);

            Suite.BillItemRepositoryMock
                .Verify(m => m.Add(It.Is<BillItem>(
                    item => item.BillId.Equals(billId)
                    && item.Key.Equals(key)
                    && item.Value.Equals(value)
                    && item.Price.Equals(price)
                    && item.Cost.Equals(cost))));

            Suite.BillItemRepositoryMock
                .Verify(m => m.Add(result));
            Suite.BillItemRepositoryMock
                .Verify(m => m.Save());

            Suite.BillServiceMock
                .Verify(m => m.SetTotalCost(billId, totalCost));

            Assert.Equal(billId, result.BillId);
            Assert.Equal(key, result.Key);
            Assert.Equal(value, result.Value);
            Assert.Equal(price, result.Price);
            Assert.Equal(cost, result.Cost);
        }

        [Fact]
        public async Task CreateBillItemWhenBillDoesNotExist()
        {
            var billId = 1;
            var key = "SomeKey";
            var value = 123;
            var price = 123.45m;

            var cost = value * price;

            Suite.BillServiceMock
                .Setup(m => m.IsExist(billId))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<EntityNotFoundException>("Bill", () => Suite.BillItemService.Create(billId, key, value, price, cost));
        }

        [Fact]
        public async Task CreateBillItemWhenCostIsNotCorrect()
        {
            var billId = 1;
            var key = "SomeKey";
            var value = 123;
            var price = 123.45m;

            var cost = 2;

            Suite.BillServiceMock
                .Setup(m => m.IsExist(billId))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>("Cost", () => Suite.BillItemService.Create(billId, key, value, price, cost));
        }
    }
}