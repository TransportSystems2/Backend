namespace TransportSystems.Backend.Core.Domain.Core.Organization
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }

        public bool IsPrivate { get; set; }
    }
}