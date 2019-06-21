namespace TransportSystems.Backend.Core.Domain.Core.Users
{
    public class IdentityUser : BaseEntity
    {
        public const string AdminRoleName = "Admin";

        public const string ModeratorRoleName = "Moderator";

        public const string DispatcherRoleName = "Dispatcher";

        public const string DriverRoleName = "Driver";

        public const string CustomerRoleName = "Customer";

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}