namespace TransportSystems.Backend.Core.Domain.Core.Users
{
    public abstract class User : BaseEntity
    {
        public int IdentityUserId { get; set; }
    }
}