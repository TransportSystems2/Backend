using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Geo;
using TransportSystems.Backend.Application.Models.Geo;

namespace TransportSystems.Backend.API.Controllers.Address
{
    [Route("api/[controller]")]
    public class AddressesController : Controller
    {
        public AddressesController(IApplicationAddressService service)
        {
            Service = service;
        }

        protected IApplicationAddressService Service { get; }

        [HttpGet("geocode")]
        public Task<ICollection<AddressAM>> Geocode(string request, int maxResult = 5)
        {
           return Service.Geocode(request, maxResult);
        }

        [HttpGet("reverse_geocode")]
        public Task<ICollection<AddressAM>> ReverseGeocode(double latitude, double longitude)
        {
            return Service.ReverseGeocode(latitude, longitude);
        }
    }
}
