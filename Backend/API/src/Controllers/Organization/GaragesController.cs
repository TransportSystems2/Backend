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
    }
}