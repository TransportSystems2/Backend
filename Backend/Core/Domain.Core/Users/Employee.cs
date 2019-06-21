namespace TransportSystems.Backend.Core.Domain.Core.Users
{
    public abstract class Employee : IdentityUser
    {
        public int CompanyId { get; set; }
    }
}