using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Transport;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public class DriverService : EmployeeService<Driver>, IDriverService
    {
        public DriverService(
            IDriverRepository repository,
            ICompanyService companyService,
            IVehicleService vehicleService)
            : base(repository, companyService)
        {
            VehicleService = vehicleService;
        }

        protected new IDriverRepository Repository => (IDriverRepository)base.Repository;

        protected IVehicleService VehicleService { get; }

        public async Task AssignVehicle(int driverId, int vehicleId)
        {
            var driver = await Repository.Get(driverId);
            if (driver == null)
            {
                throw new EntityNotFoundException($"Driver with id = {driverId}, doesn't exist", "Driver");
            }

            var vehicle = await VehicleService.Get(vehicleId);
            if (vehicle == null)
            {
                throw new EntityNotFoundException($"Vehicle with id = {vehicleId}, doesn't exist", "Vehicle");
            }

            if (!driver.CompanyId.Equals(vehicle.CompanyId))
            {
                throw new ArgumentException($"Driver id = {driverId} companyId = {driver.CompanyId} and Vehicle id = {vehicleId} companyId = {vehicle.CompanyId} belongs different companies", "Company");
            }

            if (!await CompanyService.IsExist(driver.CompanyId))
            {
                throw new EntityNotFoundException($"Company with id = {driver.CompanyId}, doesn't exist", "Company");
            }

            driver.VehicleId = vehicle.Id;

            await Repository.Update(driver);
            await Repository.Save();
        }

        public override string[] GetSpecificRoles()
        {
            return new [] { IdentityUser.DriverRoleName };
        }
    }
}