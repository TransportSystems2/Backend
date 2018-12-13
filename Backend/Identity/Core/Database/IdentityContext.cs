using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Database
{
    public class IdentityContext : IdentityDbContext<User, UserRole, int>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        public DbSet<PushToken> PushTokens { get; set; }
    }
}