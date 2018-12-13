using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;

using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Core.Business
{
    public class RoleService : RoleManager<UserRole>, IRoleService
    {
        public RoleService(
            IRoleStore<UserRole> store,
            IEnumerable<IRoleValidator<UserRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<UserRole>> logger)
            : base(
                store,
                roleValidators,
                keyNormalizer,
                errors,
                logger)
        {
        }
    }
}
