using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Billing
{
    public class BillItemService : DomainService<BillItem>, IBillItemService
    {
        public BillItemService(
            IBillItemRepository repository,
            IBillService billService)
            : base(repository)
        {
            BillService = billService;
        }

        protected new IBillItemRepository Repository => base.Repository as IBillItemRepository;

        protected IBillService BillService { get; }

        public async Task<BillItem> Create(
            int billId,
            string key,
            int value,
            decimal price,
            decimal cost)
        {
            var billItem = new BillItem
            {
                BillId = billId,
                Key = key,
                Value = value,
                Price = price,
                Cost = cost
            };

            await Create(billItem);
            await UpdateTotalCost(billId);

            return billItem;
        }

        public async Task<ICollection<BillItem>> GetAll(int billId)
        {
            if (!await BillService.IsExist(billId))
            {
                throw new ArgumentException($"BillId:{billId} is null", "Bill");
            }

            return await Repository.GetAll(billId);
        }

        public async Task UpdateTotalCost(int billId)
        {
            var totalCost = await Repository.GetTotalCost(billId);
            await BillService.SetTotalCost(billId, totalCost);
        }

        protected async Task Create(BillItem bill)
        {
            await Verify(bill);

            await Repository.Add(bill);
            await Repository.Save();
        }

        protected async override Task<bool> DoVerifyEntity(BillItem entity)
        {
            if (!await BillService.IsExist(entity.BillId))
            {
                throw new EntityNotFoundException($"BillId:{entity.BillId} doesn't exist", "Bill");
            }

            if (entity.Cost != entity.Value * entity.Price)
            {
                throw new ArgumentException($"Cost:{entity.Cost} isn't equal to the product of value:{entity.Value} and price:{entity.Price}", "Cost");
            }

            return true;
        }
    }
}