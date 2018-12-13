using Microsoft.AspNetCore.Identity;

namespace TransportSystems.Backend.Identity.Core.Data.Domain
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}