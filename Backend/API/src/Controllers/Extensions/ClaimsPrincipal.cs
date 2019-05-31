using System.Security.Claims;

namespace TransportSystems.Backend.API.Controllers.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static int? GetIdentityId(this ClaimsPrincipal principal)
        {
            var strIdentityUserId = principal.FindFirst("sub")?.Value;

            return int.TryParse(strIdentityUserId, out int result) ? result : (int?)null;
        }
    }
}