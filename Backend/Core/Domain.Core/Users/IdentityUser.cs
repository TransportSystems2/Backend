namespace TransportSystems.Backend.Core.Domain.Core.Users
{
    public class IdentityUser : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}