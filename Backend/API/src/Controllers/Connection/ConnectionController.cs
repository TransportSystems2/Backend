using Microsoft.AspNetCore.Mvc;

namespace TransportSystems.Backend.API.Controllers.Identity
{
    [Route("api/[controller]")]
    public class ConnectionController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("server is ok");
        }
    }
}
    