using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;
using TransportSystems.Backend.Identity.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Identity.Core.Database
{
    public class PushTokenRepository : Repository<PushToken>, IPushTokenRepository
    {
        public PushTokenRepository(IdentityContext context)
            : base(context)
        {
        }

        public Task<PushToken> ByValue(string value)
        {
            return Entities.SingleOrDefaultAsync(e => e.Value.Equals(value));
        }
    }
}