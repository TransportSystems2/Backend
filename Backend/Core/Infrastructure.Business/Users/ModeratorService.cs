using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public class ModeratorService : EmployeeService<Moderator>, IModeratorService
    {
        public ModeratorService(IModeratorRepository repository, IIdentityUserService identityUserService, ICompanyService companyService)
            : base(repository, identityUserService, companyService)
        {
        }

        protected new IModeratorRepository Repository => (IModeratorRepository)base.Repository;

        public override string[] GetSpecificRoles()
        {
            return new[] { UserRole.ModeratorRoleName, UserRole.EmployeeRoleName};
        }
    }
}