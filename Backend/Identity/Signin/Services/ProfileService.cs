using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using TransportSystems.Backend.Identity.Core.Data.Domain;

namespace TransportSystems.Backend.Identity.Signin.Services
{
    public class ProfileService : IProfileService
    {
        public ProfileService(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        protected UserManager<User> UserManager
        {
            get;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await UserManager.GetUserAsync(context.Subject);

            var claims = new List<Claim> {
                new Claim ("CompanyId", user.CompanyId.ToString ())
            };

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await UserManager.GetUserAsync(context.Subject);
            context.IsActive = user != null;
        }
    }
}