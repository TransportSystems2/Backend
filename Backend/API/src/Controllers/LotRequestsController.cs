using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.Core.Domain.Core.Trading;
using TransportSystems.Backend.Core.Services.Interfaces.Trading;

namespace TransportSystems.API.Controllers
{
    [Route("api/[controller]")]
    public class LotRequestsController : Controller
    {
        private ILotRequestService lotRequestService;

        public LotRequestsController(ILotRequestService lotRequestService)
        {
            this.lotRequestService = lotRequestService;
        }

        /// <summary>
        /// Список лотов
        /// </summary>
        [HttpGet]
        public IEnumerable<LotRequest> Get()
        {
            return null;
        }
    }
}
