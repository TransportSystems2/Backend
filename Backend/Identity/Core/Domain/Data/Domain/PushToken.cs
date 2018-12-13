using System;

namespace TransportSystems.Backend.Identity.Core.Domain.Data.Domain
{
    public class PushToken : BaseEntity
    {
        public string Value { get; set; }

        public PushTokenType Type { get; set; }
        
        public int UserId { get; set; }
    }
}