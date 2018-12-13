using Microsoft.AspNetCore.Identity;

namespace TransportSystems.Backend.Identity.Core.Data.Domain
{
    public class UserRole : IdentityRole<int>
    {
        public const string AdminRoleName = "admin";

        public const string UserRoleName = "user";

        public const string ModeratorRoleName = "Moderator";

        public const string DispatcherRoleName = "Dispatcher";

        public const string DriverRoleName = "driver";

        public const string CustomerRoleName = "customer";

        public const string EmployeeRoleName = "employee";

        public UserRole()
        {
        }

        public UserRole(string roleName)
            : base(roleName)
        {
        }
    }
}