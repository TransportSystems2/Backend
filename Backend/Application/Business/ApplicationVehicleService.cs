using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Catalogs;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Catalogs;
using TransportSystems.Backend.Core.Domain.Core.Transport;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;

namespace TransportSystems.Backend.Application.Business
{
    public class ApplicationVehicleService :
        ApplicationTransactionService,
        IApplicationVehicleService
    {
        public ApplicationVehicleService(
            ITransactionService transactionService,
            IVehicleService domainVehicleService,
            IApplicationCatalogService catalogService) : base(transactionService)
        {
            DomainVehicleService = domainVehicleService;
            CatalogService = catalogService;
        }

        protected IVehicleService DomainVehicleService { get; }

        protected IApplicationCatalogService CatalogService { get; }

        public Task<Vehicle> CreateDomainVehicle(int companyId, VehicleAM vehicle)
        {
            return DomainVehicleService.Create(
                companyId,
                vehicle.RegistrationNumber,
                vehicle.BrandCatalogItem.Id,
                vehicle.CapacityCatalogItem.Id,
                vehicle.KindCatalogItem.Id);
        }

        public async Task<VehicleAM> GetVehicle(int id)
        {
            var domainVehicle = await DomainVehicleService.Get(id);
            return await GetVehicle(domainVehicle);
        }

        public virtual async Task<VehicleAM> GetVehicle(Vehicle domainVehicle)
        {
            if (domainVehicle == null)
            {
                throw new ArgumentNullException("DomainVehicle");
            }

            var result = new VehicleAM();

            result.Id = domainVehicle.Id;
            result.RegistrationNumber = domainVehicle.RegistrationNumber;
            result.BrandCatalogItem = await CatalogService.GetCatalogItem(domainVehicle.BrandCatalogItemId);
            result.CapacityCatalogItem = await CatalogService.GetCatalogItem(domainVehicle.CapacityCatalogItemId);
            result.KindCatalogItem = await CatalogService.GetCatalogItem(domainVehicle.KindCatalogItemId);

            return result;
        }

        public async Task<ICollection<VehicleAM>> GetByCompany(int companyId)
        {
            var domainVehicles = await DomainVehicleService.GetByCompany(companyId);
            var result = new ConcurrentBag<VehicleAM>();
            var exceptions = new ConcurrentQueue<Exception>();

            await domainVehicles.ParallelForEachAsync(
                async domainVehicle =>
                {
                    try
                    {
                        var vehicle = await GetVehicle(domainVehicle);
                        result.Add(vehicle);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                }
            );

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return result.ToList();
        }

        public async Task<VehicleCatalogItemsAM> GetCatalogItems()
        {
            var result = new VehicleCatalogItemsAM();

            var brands = await CatalogService.GetCatalogItems(CatalogKind.Vehicle, CatalogItemKind.Brand);
            result.Brands.AddRange(brands);

            var capacities = await CatalogService.GetCatalogItems(CatalogKind.Vehicle, CatalogItemKind.Capacity);
            result.Capacities.AddRange(capacities);

            var kinds = await CatalogService.GetCatalogItems(CatalogKind.Vehicle, CatalogItemKind.Kind);
            result.Kinds.AddRange(kinds);

            return result;
        }
    }
}