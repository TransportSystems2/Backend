using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportSystems.Backend.API.Controllers.Extensions;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Models.SignUp;

namespace TransportSystems.Backend.API.Controllers.SignUp
{
    [Route("api/[controller]")]
    [Authorize(Roles = "user")]
    public class SignUpController : Controller
    {
        public SignUpController(ISignUpService signUpService)
        {
            SignUpService = signUpService;
        }

        protected ISignUpService SignUpService { get; }

        /// <summary>
        /// Регистрация компании
        /// </summary>
        /// <response code="200">Компания создана</response>
        /// <response code="400">Id пользователя не найден в claims</response> 
        [HttpPost("company")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SignUpCompany([FromBody]CompanyApplicationAM companyApplication)
        {
            var identityUserId = User.GetIdentityId();
            if (identityUserId == -1)
            {
                var problem = new ValidationProblemDetails
                {
                    Title = $"identity user id can't be parsed from claims",
                    Detail = $"identity user id can't be parsed from claims",
                    Status = 400
                };

                return base.ValidationProblem(problem);
            }

            await SignUpService.SignUpCompany(companyApplication);

            return Ok();
        }
    }
}