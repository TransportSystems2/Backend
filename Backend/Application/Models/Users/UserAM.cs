namespace TransportSystems.Backend.Application.Models.Users
{
    public class UserAM : BaseAM
    {
        public int IdentityUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
    }
}