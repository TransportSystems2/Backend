using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Data.External.PushToken
{
    public class PushTokenModel
    {
        public string Value { get; set; }

        public PushTokenType Type { get; set; }

        public int UserId { get; set; }
    }
}