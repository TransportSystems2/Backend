using System;
using Microsoft.AspNetCore.Mvc;

namespace TransportSystems.Backend.Identity.Signin.Controllers
{
    [Route("identity/ping")]
    public class ConnectionController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
