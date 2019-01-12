namespace TransportSystems.Backend.Core.Domain.Core.Users
{
    public abstract class Employee : User
    {
        public int CompanyId { get; set; }
    }
}