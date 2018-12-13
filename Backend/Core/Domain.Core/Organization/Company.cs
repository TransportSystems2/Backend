namespace TransportSystems.Backend.Core.Domain.Core.Organization
{
    public class Company : BaseEntity
    {
        public int GarageId { get; set; }

        public string Name { get; set; }

        public int ModeratorId { get; set; }

        public bool IsPrivate { get; set; }
    }
}