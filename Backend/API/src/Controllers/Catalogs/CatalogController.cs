using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.Application.Interfaces.Catalogs;

namespace TransportSystems.Backend.API.Controllers.Catalogs
{
    [Route("api/[controller]")]
    public class CatalogController : Controller
    {
		public CatalogController(IApplicationCatalogService service)
		{
            Service = service;
		}

		protected IApplicationCatalogService Service;

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("server is ok");
        }
    }
}
