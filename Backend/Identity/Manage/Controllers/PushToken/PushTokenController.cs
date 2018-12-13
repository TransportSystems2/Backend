using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.Identity.Core.Data.External.PushToken;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Manage.Controllers.PushToken
{
    [Route("identity/pushtoken")]
    public class PushTokenController : ControllerBase
    {
        public PushTokenController(IPushTokenService pushTokenService)
        {
            PushTokenService = pushTokenService;
        }

        protected IPushTokenService PushTokenService { get; }
        
        [HttpGet]
        public async Task<IActionResult> Get(int [] ids)
        {
            try
            {
                var tokens = await PushTokenService.ReadTokens(ids);
                
                return Ok(tokens);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PushTokenModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var token = await PushTokenService.CreateToken(
                    model.Value,
                    model.Type,
                    model.UserId);

                return Ok(token);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] PushTokenModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await PushTokenService.DeleteToken(
                    model.Value,
                    model.Type,
                    model.UserId);

                return Ok(result);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
