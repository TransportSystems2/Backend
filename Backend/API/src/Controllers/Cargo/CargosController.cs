using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.Transport;

namespace TransportSystems.Backend.API.Controllers.Cargo
{
    [Route("api/[controller]")]
    public class CargosController : Controller
    {
        public CargosController(IApplicationCargoService service)
        {
            Service = service;
        }

        protected IApplicationCargoService Service;

        [HttpGet("catalog_items")]
        public Task<CargoCatalogItemsAM> GetCatalogItems()
        {
            return Service.GetCatalogItems();
        }

        [HttpGet("valid_registration_number")]
        public Task<bool> ValidRegistrationNumber(string registrationNumber)
        {
            return Service.ValidRegistrationNumber(registrationNumber);
        }
    }
}