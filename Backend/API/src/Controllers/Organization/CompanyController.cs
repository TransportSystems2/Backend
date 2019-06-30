using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.API.Controllers.Organization
{
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : Controller
    {
        public CompanyController(IApplicationCompanyService companyService)
        {
            Service = companyService;
        }

        protected IApplicationCompanyService Service { get; }

        [HttpGet("getDrivers")]
        public async Task<ICollection<DriverAM>> GetDrivers()
        {
            var companyId = User.FindFirst("companyId").Value;
            var result = await Service.GetDrivers(int.Parse(companyId));

            return result;
        }
    }
}