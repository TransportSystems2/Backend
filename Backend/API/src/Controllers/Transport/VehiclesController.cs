using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.API.Controllers.Transport
{
    [Route("api/[controller]")]
    [Authorize(Roles = "user")]
    public class VehiclesController : Controller
    {
        public VehiclesController(IApplicationVehicleService ApplicationVehicleService)
        {
            ApplicationVehicleService = ApplicationVehicleService;
        }

        protected IApplicationVehicleService ApplicationVehicleService { get; }

        [HttpGet("catalog_items")]
        public Task<VehicleCatalogItemsAM> GetCatalogItems()
        {
            return ApplicationVehicleService.GetCatalogItems();
        }
    }
}