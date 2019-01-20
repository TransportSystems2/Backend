using Common.Models.Units;
using System;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Billing;

namespace TransportSystems.Backend.Application.Business.Billing
{
    public class ApplicationBillService :
        ApplicationTransactionService,
        IApplicationBillService
    {
        public ApplicationBillService(
            ITransactionService transactionService,
            IBillService domainBillService,
            IBillItemService domainBillItemService,
            IBasketService domainBasketService,
            IApplicationPricelistService pricelistService,
            IApplicationCityService cityService)
            : base(transactionService)
        {
            DomainBillService = domainBillService;
            DomainBillItemService = domainBillItemService;
            DomainBasketService = domainBasketService;
            PricelistService = pricelistService;
            CityService = cityService; 
        }

        protected IBillService DomainBillService { get; }

        protected IBillItemService DomainBillItemService { get; }

        protected IBasketService DomainBasketService { get; }

        protected IApplicationPricelistService PricelistService { get; }

        protected IApplicationCityService CityService { get; }

        public async Task<BillAM> CalculateBill(BillInfoAM billInfo, BasketAM basket)
        {
            var domainPrice = await PricelistService.GetDomainPrice(billInfo.PriceId);

            if (domainPrice == null)
            {
                throw new EntityNotFoundException($"PriceId:{billInfo.PriceId} doesn't exist", "Price");
            }

            var result = new BillAM
            {
                Info = billInfo
            };

            if (basket.Distance.ToMeters() > 0)
            {
                var billItem = await CalculateBillItem(BillItem.MetersBillKey, (int)basket.Distance.ToMeters(), domainPrice.PerMeter, billInfo.DegreeOfDifficulty);
                result.Items.Add(billItem);
            }

            if (basket.LoadingValue > 0)
            {
                var billItem = await CalculateBillItem(BillItem.LoadingBillKey, basket.LoadingValue, domainPrice.Loading, billInfo.DegreeOfDifficulty);
                result.Items.Add(billItem);
            }

            if (basket.LockedSteeringValue > 0)
            {
                var billItem = await CalculateBillItem(BillItem.LockedSteeringKey, basket.LockedSteeringValue, domainPrice.LockedSteering, billInfo.DegreeOfDifficulty);
                result.Items.Add(billItem);
            }

            if (basket.LockedWheelsValue > 0)
            {
                var billItem = await CalculateBillItem(BillItem.LockedWheelsKey, basket.LockedWheelsValue, domainPrice.LockedWheel, billInfo.DegreeOfDifficulty);
                result.Items.Add(billItem);
            }

            if (basket.OverturnedValue > 0)
            {
                var billItem = await CalculateBillItem(BillItem.OverturnedKey, basket.OverturnedValue, domainPrice.Overturned, billInfo.DegreeOfDifficulty);
                result.Items.Add(billItem);
            }

            if (basket.DitchValue > 0)
            {
                var billItem = await CalculateBillItem(BillItem.DitchKey, basket.DitchValue, domainPrice.Ditch, billInfo.DegreeOfDifficulty);
                result.Items.Add(billItem);
            }

            result.TotalCost = result.Items.Select(i => i.Cost).Sum();

            return result;
        }

        public Task<BillItemAM> CalculateBillItem(string key, int value, decimal price, float degreeOfDifficulty)
        {
            var adjustedPrice = price * (decimal)degreeOfDifficulty;
            var cost = value * adjustedPrice;

            var result = new BillItemAM
            {
                Key = key,
                Value = value,
                Price = adjustedPrice,
                Cost = cost
            };

            return Task.FromResult(result);
        }

        public async Task<Bill> CreateDomainBill(BillInfoAM billInfo, BasketAM basket)
        {
            var bill = await CalculateBill(billInfo, basket);

            var domainBasket = await DomainBasketService.Create(
                basket.Distance,
                basket.LoadingValue,
                basket.LockedSteeringValue,
                basket.LockedWheelsValue,
                basket.OverturnedValue,
                basket.DitchValue);

            var domainBill = await DomainBillService.Create(
                billInfo.PriceId,
                domainBasket.Id,
                billInfo.CommissionPercentage,
                billInfo.DegreeOfDifficulty);

            foreach(var item in bill.Items)
            {
                await CreateDomainBillItem(domainBill.Id, item);
            }

            // обновим счет, т.к. только после добавления элеметов счета у него появится итоговая стоимость
            domainBill = await DomainBillService.Get(domainBill.Id);
            if (domainBill.TotalCost != bill.TotalCost)
            {
                throw new ArgumentException(
                    $"TotalCost:{domainBill.TotalCost} of domain bill isn't equal TotalCost:{bill.TotalCost} of calculated bill",
                    "TotalCost");
            }

            return domainBill;
        }

        public Task<BillItem> CreateDomainBillItem(int billId, BillItemAM billItem)
        {
            return DomainBillItemService.Create(billId, billItem.Key, billItem.Value, billItem.Price, billItem.Cost);
        }

        public async Task<BillInfoAM> GetBillInfo(Coordinate coordinate, int catalogItemId)
        {
            var domainCity = await CityService.GetDomainCityByCoordinate(coordinate);
            var domainPrice = await PricelistService.GetDomainPrice(domainCity.PricelistId, catalogItemId);

            return new BillInfoAM
            {
                PriceId = domainPrice.Id,
                CommissionPercentage = domainPrice.CommissionPercentage
            };
        }
    }
}