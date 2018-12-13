using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TransportSystems.Backend.API.Controllers.Identity
{
    /// <summary>
    /// контроллер проверки авторизации
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        /// <summary>
        /// Клэймы пользователя
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
