using System;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;
using TransportSystems.Backend.Core.Infrastructure.Business.Extension;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Billing;
using TransportSystems.Backend.Core.Services.Interfaces.Pricing;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Billing
{
    public class BillService : DomainService<Bill>, IBillService
    {
        public const float MaxDegreeOfDifficulty = 3;
        public const float MinDegreeOfDifficulty = 0.1f;

        public BillService(
            IBillRepository repository,
            IPriceService priceService,
            IBasketService basketService)
            : base(repository)
        {
            PriceService = priceService;
            BasketService = basketService;
        }

        protected new IBillRepository Repository => base.Repository as IBillRepository;

        protected IPriceService PriceService { get; }

        protected IBasketService BasketService { get; }

        public async Task<Bill> Create(
            int priceId,
            int basketId,
            byte commissionPercentage,
            float degreeOfDifficulty)
        {
            var bill = new Bill
            {
                PriceId = priceId,
                BasketId = basketId,
                CommissionPercentage = commissionPercentage,
                DegreeOfDifficulty = degreeOfDifficulty
            };

            await Create(bill);

            return bill;
        }

        public async Task SetTotalCost(int billId, decimal totalCost)
        {
            var bill = await Repository.Get(billId);

            bill.TotalCost = totalCost;

            await Repository.Update(bill);
            await Repository.Save();
        }

        protected async Task Create(Bill bill)
        {
            await Verify(bill);

            await Repository.Add(bill);
            await Repository.Save();
        }

        protected override async Task<bool> DoVerifyEntity(Bill entity)
        {
            if (!await PriceService.IsExist(entity.PriceId))
            {
                throw new EntityNotFoundException($"PriceId:{entity.PriceId} doesn't exist.", "Price");
            }

            if (!await BasketService.IsExist(entity.BasketId))
            {
                throw new EntityNotFoundException($"BasketId:{entity.BasketId} doesn't exist.", "Basket");
            }

            if (!Enumerable.Range(0, 100).Contains(entity.CommissionPercentage))
            {
                throw new ArgumentOutOfRangeException(
                    "CommissionPercentage", 
                    $"The available range for percente is 0 to 100. CommissionPercentage:{entity.CommissionPercentage} is out of range.");
            }

            if (!entity.DegreeOfDifficulty.InRange(MinDegreeOfDifficulty, MaxDegreeOfDifficulty))
            {
                throw new ArgumentOutOfRangeException(
                    "DegreeOfDifficulty",
                    $"The available range for degreeOfDifficulty is {MinDegreeOfDifficulty} to {MaxDegreeOfDifficulty}. DegreeOfDifficulty:{entity.DegreeOfDifficulty} is out of range.");
            }

            return true;
        }
    }
}