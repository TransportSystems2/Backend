using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.Application.Interfaces.Organization;

namespace TransportSystems.Backend.API.Controllers.Organization
{
    [Route("api/[controller]")]
    [Authorize(Roles = "user")]
    public class GaragesController : Controller
    {
        public GaragesController(IApplicationGarageService garageService)
        {
            GarageService = garageService;
        }

        protected IApplicationGarageService GarageService { get; }

        [HttpGet("available_provinces")]
        public Task<ICollection<string>> GetAvailableProvinces(string country)
        {
            return GarageService.GetAvailableProvinces(country);
        }

        [HttpGet("available_localities")]
        public Task<ICollection<string>> GetAvailableLocalities(string country, string province)
        {
            return GarageService.GetAvailableLocalities(country, province);
        }

        [HttpGet("available_districts")]
        public Task<ICollection<string>> GetAvailableDistricts(string country, string province, string locality)
        {
            return GarageService.GetAvailableDistricts(country, province, locality);
        }
    }
}