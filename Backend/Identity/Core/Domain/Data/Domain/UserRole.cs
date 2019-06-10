using Microsoft.AspNetCore.Identity;

namespace TransportSystems.Backend.Identity.Core.Data.Domain
{
    public class UserRole : IdentityRole<int>
    {
        public const string AdminRoleName = "Admin";

        public const string ModeratorRoleName = "Moderator";

        public const string DispatcherRoleName = "Dispatcher";

        public const string DriverRoleName = "Driver";

        public const string CustomerRoleName = "Customer";


        public UserRole()
        {
        }

        public UserRole(string roleName)
            : base(roleName)
        {
        }
    }
}