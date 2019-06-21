using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
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

        public override string GetDefaultRole()
        {
            return IdentityUser.DriverRoleName;
        }
    }
}