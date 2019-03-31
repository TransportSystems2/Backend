using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public class ModeratorService : EmployeeService<Moderator>, IModeratorService
    {
        public ModeratorService(
            IModeratorRepository repository,
            IIdentityUserService identityUserService,
            ICompanyService companyService)
            : base(repository, identityUserService, companyService)
        {
        }

        protected new IModeratorRepository Repository => (IModeratorRepository)base.Repository;

        public override async Task<string[]> GetSpecificRoles()
        {
            var baseSpecificRoles = await base.GetSpecificRoles();
            var specificRoles = new List<string>(baseSpecificRoles)
            {
                UserRole.ModeratorRoleName
            };

            return specificRoles.ToArray();
        }
    }
}