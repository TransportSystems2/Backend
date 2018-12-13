using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        /// Регистрация диспетчера
        /// </summary>
        /// <response code="200">Диспетчер создан</response>
        /// <response code="400">Id пользователя не найден в claims</response> 
        [HttpPost("dispatcher")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SignUpDispatcherCompany([FromBody]DispatcherCompanyAM dispatcherCompanyModel)
        {
            var identityUserId = GetIdentityUserId();
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

            var phoneNumber = GetIdentityUserPhoneNumber();

            dispatcherCompanyModel.Dispatcher.IdentityUserId = identityUserId;
            dispatcherCompanyModel.Dispatcher.PhoneNumber = phoneNumber;

            await SignUpService.SignUpDispatcherCompany(dispatcherCompanyModel);

            return Ok();
        }

        /// <summary>
        /// Регистрация водителя
        /// </summary>
        /// <response code="200">Диспетчер создан</response>
        /// <response code="400">Id пользователя не найден в claims</response> 
        [HttpPost("driver")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SignUpDriverCompany([FromBody]DriverCompanyAM driverCompanyModel)
        {
            var identityUserId = GetIdentityUserId();
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

            var phoneNumber = GetIdentityUserPhoneNumber();

            driverCompanyModel.Driver.IdentityUserId = identityUserId;
            driverCompanyModel.Driver.PhoneNumber = phoneNumber;

            await SignUpService.SignUpDriverCompany(driverCompanyModel);

            return Ok();
        }

        private int GetIdentityUserId()
        {
            var strIdentityUserId = User.FindFirst("sub")?.Value;

            return int.Parse(strIdentityUserId);
        }

        private string GetIdentityUserPhoneNumber()
        {
            return User.FindFirst("phone_number")?.Value;
        }
    }
}