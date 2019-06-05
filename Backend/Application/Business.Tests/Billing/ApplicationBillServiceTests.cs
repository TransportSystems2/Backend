using Common.Models.Units;
using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Pricing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Application.Business.Billing;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using Xunit;
using DotNetDistance;
using TransportSystems.Backend.Application.Models.Geo;
using TransportSystems.Backend.Application.Interfaces.Mapping;

namespace TransportSystems.Backend.Application.Business.Tests.Billing
{
    public class ApplicationBillServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationBillServiceTestSuite()
        {
            DomainBillServiceMock = new Mock<IBillService>();
            DomainBillItemServiceMock = new Mock<IBillItemService>();
            DomainBasketServiceMock = new Mock<IBasketService>();
            PricelistServiceMock = new Mock<IApplicationPricelistService>();

            Service = new ApplicationBillService(
                TransactionServiceMock.Object,
                MappingService,
                DomainBillServiceMock.Object,
                DomainBillItemServiceMock.Object,
                DomainBasketServiceMock.Object,
                PricelistServiceMock.Object);
        }

        public IApplicationBillService Service { get; }

        public Mock<IBillService> DomainBillServiceMock { get; }

        public Mock<IBillItemService> DomainBillItemServiceMock { get; }

        public Mock<IBasketService> DomainBasketServiceMock { get; }

        public Mock<IApplicationPricelistService> PricelistServiceMock { get; }
    }

    public class ApplicationBillServiceTests : BaseServiceTests<ApplicationBillServiceTestSuite>
    {
        [Fact]
        public async Task CreateDomainBill()
        {
            var commonId = 1;

            var billInfo = new BillInfoAM
            {
                PriceId = 123,
                CommissionPercentage = 10,
                DegreeOfDifficulty = 1.3f
            };

            var basket = new BasketAM
            {
                LockedWheelsValue = 1,
                Distance = Distance.FromKilometers(200),
                DitchValue = 1,
                LoadingValue = 1,
                OverturnedValue = 1,
                LockedSteeringValue = 1
            };

            var domainBasket = new Basket { Id = commonId++ };
            var domainBill = new Bill
            {
                Id = commonId++,
                PriceId = billInfo.PriceId,
                CommissionPercentage = billInfo.CommissionPercentage,
                DegreeOfDifficulty = billInfo.DegreeOfDifficulty
            };

            var domainPrice = new Price
            {
                Id = billInfo.PriceId,
                PerMeter = 0.04m,
                Loading = 900,
                LockedSteering = 300,
                LockedWheel = 400,
                Ditch = 1000,
                Overturned = 1200
            };

            var perMeterPrice = domainPrice.PerMeter * (decimal)billInfo.DegreeOfDifficulty;
            var metersCost = (int)basket.Distance.ToMeters() * perMeterPrice;

            var loadingPrice = domainPrice.Loading * (decimal)billInfo.DegreeOfDifficulty;
            var loadingCost = basket.LoadingValue * loadingPrice;

            var lockedSteeringPrice = domainPrice.LockedSteering * (decimal)billInfo.DegreeOfDifficulty;
            var lockedSteeringCost = basket.LockedSteeringValue * lockedSteeringPrice;

            var lockedWheelPrice = domainPrice.LockedWheel * (decimal)billInfo.DegreeOfDifficulty;
            var lockedWheelsCost = basket.LockedWheelsValue * lockedWheelPrice;

            var overturnedPrice = domainPrice.Overturned * (decimal)billInfo.DegreeOfDifficulty;
            var overturnedCost = basket.OverturnedValue * overturnedPrice;

            var ditchPrice = domainPrice.Ditch * (decimal)billInfo.DegreeOfDifficulty;
            var ditchCost = basket.DitchValue * ditchPrice;

            var bill = new BillAM
            {
                Info = billInfo,
                Basket = basket,
                TotalCost = metersCost + loadingCost + lockedSteeringCost + lockedWheelsCost + overturnedCost + ditchCost,
                Items =
                {
                    new BillItemAM { Key = BillItem.MetersBillKey, Price = perMeterPrice, Value = (int)basket.Distance.ToMeters(), Cost = metersCost },
                    new BillItemAM { Key = BillItem.LoadingBillKey, Price = loadingPrice, Value = basket.LoadingValue, Cost = loadingCost },
                    new BillItemAM { Key = BillItem.LockedSteeringKey, Price = lockedSteeringPrice, Value = basket.LockedSteeringValue, Cost = lockedSteeringCost },
                    new BillItemAM { Key = BillItem.LockedWheelsKey, Price = lockedWheelPrice, Value = basket.LockedWheelsValue, Cost = lockedWheelsCost },
                    new BillItemAM { Key = BillItem.OverturnedKey, Price = overturnedPrice, Value = basket.OverturnedValue, Cost = overturnedCost },
                    new BillItemAM { Key = BillItem.DitchKey, Price = ditchPrice, Value = basket.DitchValue, Cost = ditchCost }
                }
            };

            Suite.DomainBasketServiceMock
                .Setup(m => m.Create(
                    basket.Distance,
                    basket.LoadingValue,
                    basket.LockedSteeringValue,
                    basket.LockedWheelsValue,
                    basket.OverturnedValue,
                    basket.DitchValue))
                .ReturnsAsync(domainBasket);

            Suite.PricelistServiceMock
                .Setup(m => m.GetDomainPrice(billInfo.PriceId))
                .ReturnsAsync(domainPrice);
            Suite.DomainBillServiceMock
                .Setup(m => m.Create(
                    billInfo.PriceId,
                    domainBasket.Id,
                    billInfo.CommissionPercentage,
                    billInfo.DegreeOfDifficulty))
                .ReturnsAsync(domainBill);
            Suite.DomainBillServiceMock
                .Setup(m => m.Get(domainBill.Id))
                .Returns(() => { domainBill.TotalCost = (decimal)(11800 * billInfo.DegreeOfDifficulty); return Task.FromResult(domainBill); });

            var result = await Suite.Service.CreateDomainBill(bill);

            Suite.DomainBillItemServiceMock
                .Verify(m => m.Create(
                    domainBill.Id,
                    BillItem.MetersBillKey,
                    (int)basket.Distance.ToMeters(),
                    perMeterPrice,
                    metersCost));

            Suite.DomainBillItemServiceMock
                .Verify(m => m.Create(
                    domainBill.Id,
                    BillItem.LoadingBillKey,
                    basket.LoadingValue,
                    loadingPrice,
                    loadingCost));

            Suite.DomainBillItemServiceMock
                .Verify(m => m.Create(
                    domainBill.Id,
                    BillItem.LockedSteeringKey,
                    basket.LockedSteeringValue,
                    lockedSteeringPrice,
                    lockedSteeringCost));

            Suite.DomainBillItemServiceMock
                .Verify(m => m.Create(
                    domainBill.Id,
                    BillItem.LockedWheelsKey,
                    basket.LockedWheelsValue,
                    lockedWheelPrice,
                    lockedWheelsCost));

            Suite.DomainBillItemServiceMock
                .Verify(m => m.Create(
                    domainBill.Id,
                    BillItem.OverturnedKey,
                    basket.OverturnedValue,
                    overturnedPrice,
                    overturnedCost));

             Suite.DomainBillItemServiceMock
                .Verify(m => m.Create(
                    domainBill.Id,
                    BillItem.DitchKey,
                    basket.DitchValue,
                    ditchPrice,
                    ditchCost));

            // Надо получить счет заного, чтобы в нем была итоговая сумма
            Suite.DomainBillServiceMock
                .Verify(m => m.Get(domainBill.Id));
        }

        [Fact]
        public async Task CreateDomainBillItem()
        {
            var domainBillId = 1;

            var billItem = new BillItemAM
            {
                Key = "SomeKey",
                Value = 123,
                Price = 123.45m,
                Cost = 15184.35m
            };

            var result = await Suite.Service.CreateDomainBillItem(domainBillId, billItem);

            Suite.DomainBillItemServiceMock
                .Verify(m => m.Create(domainBillId, billItem.Key, billItem.Value, billItem.Price, billItem.Cost));
        }

        [Fact]
        public async Task CalculateBill()
        {
            var domainPrice = new Price
            {
                Id = 123,
                PerMeter = 40m,
                Loading = 900m,
                LockedSteering = 400m,
                LockedWheel = 300m,
                Overturned = 1500m,
                Ditch = 1000m
            };

            var billInfo = new BillInfoAM
            {
                PriceId = domainPrice.Id,
                CommissionPercentage = 10,
                DegreeOfDifficulty = 1.2f
            };

            var basket = new BasketAM
            {
                LockedWheelsValue = 1,
                Distance = Distance.FromKilometers(200),
                DitchValue = 1,
                LoadingValue = 1,
                OverturnedValue = 1,
                LockedSteeringValue = 1
            };

            Suite.PricelistServiceMock
                .Setup(m => m.GetDomainPrice(billInfo.PriceId))
                .ReturnsAsync(domainPrice);

            var result = await Suite.Service.CalculateBill(billInfo, basket);

            Assert.Equal(billInfo, result.Info);
            Assert.Equal(6, result.Items.Count);

            var kmBillItem = result.Items[0];
            var perMeterPrice = domainPrice.PerMeter * (decimal)billInfo.DegreeOfDifficulty;
            var kmCost = (int)basket.Distance.ToMeters() * perMeterPrice;
            Assert.Equal(BillItem.MetersBillKey, kmBillItem.Key);
            Assert.Equal(basket.Distance.ToMeters(), kmBillItem.Value);
            Assert.Equal(perMeterPrice, kmBillItem.Price);
            Assert.Equal(kmCost, kmBillItem.Cost);

            var loadingBillItem = result.Items[1];
            var loadingPrice = domainPrice.Loading * (decimal)billInfo.DegreeOfDifficulty;
            var loadingCost = basket.LoadingValue * loadingPrice;
            Assert.Equal(BillItem.LoadingBillKey, loadingBillItem.Key);
            Assert.Equal(basket.LoadingValue, loadingBillItem.Value);
            Assert.Equal(loadingPrice, loadingBillItem.Price);
            Assert.Equal(loadingCost, loadingBillItem.Cost);

            var lockedSteeringBillItem = result.Items[2];
            var lockedSteeringPrice = domainPrice.LockedSteering * (decimal)billInfo.DegreeOfDifficulty;
            var lockedSteeringCost = basket.LockedSteeringValue * lockedSteeringPrice;
            Assert.Equal(BillItem.LockedSteeringKey, lockedSteeringBillItem.Key);
            Assert.Equal(basket.LockedSteeringValue, lockedSteeringBillItem.Value);
            Assert.Equal(lockedSteeringPrice, lockedSteeringBillItem.Price);
            Assert.Equal(lockedSteeringCost, lockedSteeringBillItem.Cost);

            var lockedWheelBillItem = result.Items[3];
            var lockedWheelPrice = domainPrice.LockedWheel * (decimal)billInfo.DegreeOfDifficulty;
            var lockedWheelsCost = basket.LockedWheelsValue * lockedWheelPrice;
            Assert.Equal(BillItem.LockedWheelsKey, lockedWheelBillItem.Key);
            Assert.Equal(basket.LockedWheelsValue, lockedWheelBillItem.Value);
            Assert.Equal(lockedWheelPrice, lockedWheelBillItem.Price);
            Assert.Equal(lockedWheelsCost, lockedWheelBillItem.Cost);

            var overturnedBillItem = result.Items[4];
            var overturnedPrice = domainPrice.Overturned * (decimal)billInfo.DegreeOfDifficulty;
            var overturnedCost = basket.OverturnedValue * overturnedPrice;
            Assert.Equal(BillItem.OverturnedKey, overturnedBillItem.Key);
            Assert.Equal(basket.OverturnedValue, overturnedBillItem.Value);
            Assert.Equal(overturnedPrice, overturnedBillItem.Price);
            Assert.Equal(overturnedCost, overturnedBillItem.Cost);

            var ditchBillItem = result.Items[5];
            var ditchPrice = domainPrice.Ditch * (decimal)billInfo.DegreeOfDifficulty;
            var ditchCost = basket.DitchValue * ditchPrice;
            Assert.Equal(BillItem.DitchKey, ditchBillItem.Key);
            Assert.Equal(basket.DitchValue, ditchBillItem.Value);
            Assert.Equal(ditchPrice, ditchBillItem.Price);
            Assert.Equal(ditchCost, ditchBillItem.Cost);

            var totalCost = kmCost + loadingCost + lockedSteeringCost + lockedWheelsCost + overturnedCost + ditchCost;
            Assert.Equal(totalCost, result.TotalCost);
        }

        [Fact]
        public async Task CalculateBillWhenInBasketOnlyMetersAndLoading()
        {
            var domainPrice = new Price
            {
                Id = 123,
                PerMeter = 40m,
                Loading = 900m,
                LockedSteering = 400m,
                LockedWheel = 300m,
                Overturned = 1500m,
                Ditch = 1000m
            };

            var billInfo = new BillInfoAM
            {
                PriceId = domainPrice.Id,
                CommissionPercentage = 10,
                DegreeOfDifficulty = 1.2f
            };

            var basket = new BasketAM
            {
                LockedWheelsValue = 0,
                Distance = Distance.FromKilometers(200),
                LoadingValue = 1,
                DitchValue = 0,
                OverturnedValue = 0,
                LockedSteeringValue = 0
            };

            Suite.PricelistServiceMock
                .Setup(m => m.GetDomainPrice(billInfo.PriceId))
                .ReturnsAsync(domainPrice);

            var result = await Suite.Service.CalculateBill(billInfo, basket);

            Assert.Equal(billInfo, result.Info);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task CalculateBillWhenPriceDoesNotExist()
        {
            var domainPriceId = 1;

            var billInfo = new BillInfoAM
            {
                PriceId = domainPriceId
            };

            var basket = new BasketAM();

            Suite.PricelistServiceMock
                .Setup(m => m.GetDomainPrice(billInfo.PriceId))
                .Returns(Task.FromResult<Price>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>("Price", () => Suite.Service.CalculateBill(billInfo, basket));
        }

        [Fact]
        public async Task CalculateBillItem()
        {
            var key = "SomeKey";
            var value = 123;
            var price = 123.45m;
            var degreeOfDifficulty = 1.1f;

            var result = await Suite.Service.CalculateBillItem(key, value, price, degreeOfDifficulty);

            var adjustedPrice = price * (decimal)degreeOfDifficulty;
            var cost = value * adjustedPrice;

            Assert.Equal(key, result.Key);
            Assert.Equal(value, result.Value);
            Assert.Equal(adjustedPrice, result.Price);
            Assert.Equal(cost, result.Cost);
        }

        [Fact]
        public async Task GetBillInfo()
        {
            var commonId = 1;

            var catalogItemId = commonId++;
            var domainPricelistId = commonId++;

            var domainPrice = new Price
            {
                Id = commonId++,
                CommissionPercentage = 10
            };


            Suite.PricelistServiceMock
                .Setup(m => m.GetDomainPrice(domainPricelistId, catalogItemId))
                .ReturnsAsync(domainPrice);

            var result = await Suite.Service.GetDefaultBillInfo(domainPricelistId, catalogItemId);

            Assert.Equal(domainPrice.Id, result.PriceId);
            Assert.Equal(domainPrice.CommissionPercentage, result.CommissionPercentage);
            Assert.Equal(Bill.DefaultDegreeOfDifficulty, result.DegreeOfDifficulty);
        }

        [Fact]
        public async Task GetTotalCost()
        {
            var commonId = 1;

            var domainBill = new Bill
            {
                Id = commonId++,
                TotalCost = 100m
            };

            Suite.DomainBillServiceMock
                .Setup(m => m.Get(domainBill.Id))
                .ReturnsAsync(domainBill);

            var result = await Suite.Service.GetTotalCost(domainBill.Id);

            Assert.Equal(domainBill.TotalCost, result);
        }

        [Fact]
        public async Task GetTotalCostWhenBillIdDoesNotExist()
        {
            var notexistentBillId = 1;

            Suite.DomainBillServiceMock
                .Setup(m => m.Get(notexistentBillId))
                .Returns(Task.FromResult<Bill>(null));

            await Assert.ThrowsAsync<EntityNotFoundException>("Bill", () => Suite.Service.GetTotalCost(notexistentBillId));
        }
    }
}