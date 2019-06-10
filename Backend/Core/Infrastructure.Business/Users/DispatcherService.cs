using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public class DispatcherService : EmployeeService<Dispatcher>, IDispatcherService
    {
        public DispatcherService(IDispatcherRepository repository, ICompanyService companyService)
            : base(repository, companyService)
        {
        }

        protected new IDispatcherRepository Repository => (IDispatcherRepository)base.Repository;

        public override string[] GetSpecificRoles()
        {
            return new [] { IdentityUser.DispatcherRoleName };
        }
    }
}