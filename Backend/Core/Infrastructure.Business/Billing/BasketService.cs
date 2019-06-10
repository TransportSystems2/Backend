using DotNetDistance;
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
    public class BasketService : DomainService<Basket>, IBasketService
    {
        public BasketService(
            IBasketRepository repository)
            : base(repository)
        {
        }

        protected new IBasketRepository Repository => base.Repository as IBasketRepository;

        public async Task<Basket> Create(
            Distance distance,
            int loadingValue,
            int lockedSteeringValue,
            int lockedWheelsValue,
            int overturnedValue,
            int ditchValue)
        {
            var basket = new Basket
            {
                Distance = distance,
                LoadingValue = loadingValue,
                LockedSteeringValue = lockedSteeringValue,
                LockedWheelsValue = lockedWheelsValue,
                OverturnedValue = overturnedValue,
                DitchValue = ditchValue
            };

            await Create(basket);

            return basket;
        }

        protected async Task Create(Basket basket)
        {
            await Verify(basket);

            await Repository.Add(basket);
            await Repository.Save();
        }

        protected override Task<bool> DoVerifyEntity(Basket entity)
        {
            return Task.FromResult(true);
        }
    }
}