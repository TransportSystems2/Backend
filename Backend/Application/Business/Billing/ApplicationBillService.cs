﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Billing;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Interfaces.Pricing;
using TransportSystems.Backend.Application.Models.Billing;
using TransportSystems.Backend.Application.Interfaces.Mapping;

namespace TransportSystems.Backend.Application.Business.Billing
{
    public class ApplicationBillService :
        ApplicationTransactionService,
        IApplicationBillService
    {
        public ApplicationBillService(
            ITransactionService transactionService,
            IMappingService mappingService,
            IBillService domainBillService,
            IBillItemService domainBillItemService,
            IBasketService domainBasketService,
            IApplicationPricelistService pricelistService)
            : base(transactionService)
        {
            MappingService = mappingService;
            DomainBillService = domainBillService;
            DomainBillItemService = domainBillItemService;
            DomainBasketService = domainBasketService;
            PricelistService = pricelistService;
        }

        protected IMappingService MappingService { get; }

        protected IBillService DomainBillService { get; }

        protected IBillItemService DomainBillItemService { get; }

        protected IBasketService DomainBasketService { get; }

        protected IApplicationPricelistService PricelistService { get; }

        protected IApplicationGarageService GarageService { get; }

        public async Task<BillAM> CalculateBill(BillInfoAM billInfo, BasketAM basket)
        {
            var domainPrice = await PricelistService.GetDomainPrice(billInfo.PriceId);

            if (domainPrice == null)
            {
                throw new EntityNotFoundException($"PriceId:{billInfo.PriceId} doesn't exist", "Price");
            }

            var result = new BillAM
            {
                Info = billInfo,
                Basket = basket
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

        public async Task<Bill> CreateDomainBill(BillAM bill)
        {
            var basket = bill.Basket;
            var billInfo = bill.Info;

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

        public async Task<BillAM> GetBill(int billId)
        {
            var domainBill = await DomainBillService.Get(billId);
            if (domainBill == null)
            {
                throw new ArgumentException($"BillId:{billId} is null", "Bill");
            }

            var billInfo = new BillInfoAM
            {
                PriceId = domainBill.PriceId,
                CommissionPercentage = domainBill.CommissionPercentage,
                DegreeOfDifficulty = domainBill.DegreeOfDifficulty
            };

            var domainBasket = await DomainBasketService.Get(domainBill.BasketId);
            var basket = MappingService.Map<BasketAM>(domainBasket);

            var result = new BillAM
            {
                TotalCost = domainBill.TotalCost,
                Info = billInfo,
                Basket = basket,
            };

            var domainBillItems = await DomainBillItemService.GetAll(billId);
            foreach(var domainBillItem in domainBillItems)
            {
                var billItem = MappingService.Map<BillItemAM>(domainBillItem);
                result.Items.Add(billItem);
            }

            return result;
        }

        public async Task<BillInfoAM> GetDefaultBillInfo(int pricelistId, int catalogItemId)
        {
            var domainPrice = await PricelistService.GetDomainPrice(pricelistId, catalogItemId);

            return new BillInfoAM
            {
                PriceId = domainPrice.Id,
                CommissionPercentage = domainPrice.CommissionPercentage
            };
        }

        public async Task<decimal> GetTotalCost(int billId)
        {
            var domainBill = await DomainBillService.Get(billId);

            if (domainBill == null)
            {
                throw new EntityNotFoundException($"BillId:{billId} doesn't exist.", "Bill");
            }

            return domainBill.TotalCost;
        }
    }
}