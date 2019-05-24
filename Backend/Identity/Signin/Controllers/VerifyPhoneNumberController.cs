using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Signin.Models;
using TransportSystems.Backend.Identity.Signin.Services;

namespace TransportSystems.Backend.Identity.Signin.Controllers
{
    [Route("identity/code")]
    public class VerifyPhoneNumberController : ControllerBase
    {
        private readonly IHostingEnvironment _env;
        private readonly ISmsService _smsService;
        private readonly ISlackService _slackService;
        private readonly DataProtectorTokenProvider<User> _dataProtectorTokenProvider;
        private readonly PhoneNumberTokenProvider<User> _phoneNumberTokenProvider;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<VerifyPhoneNumberController> _logger;

        public VerifyPhoneNumberController(
            IHostingEnvironment env,
            ISmsService smsService,
            ISlackService slackService,
            DataProtectorTokenProvider<User> dataProtectorTokenProvider,
            PhoneNumberTokenProvider<User> phoneNumberTokenProvider,
            UserManager<User> userManager,
            ILogger<VerifyPhoneNumberController> logger)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _slackService = slackService ?? throw new ArgumentNullException(nameof(slackService));
            _dataProtectorTokenProvider = dataProtectorTokenProvider ?? throw new ArgumentNullException(nameof(dataProtectorTokenProvider));
            _phoneNumberTokenProvider = phoneNumberTokenProvider ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Повторная отправка кода по sms.
        /// </summary>
        /// <param name="resendToken">
        /// Токен переотправки, если токен не совпадает с ранее выданным,
        /// sms не будет отправлено.
        /// </param>
        /// <example>
        /// CfDJ8BNpUHesMKFIvvs7...
        /// </example>
        /// <param name="phoneModel">Модель телефона пользователя.</param>
        /// <response code="202">Отправлено.</response>
        /// <response code="400">Проверьте заполненую форму.</response>
        /// <response code="500">Ошибка отправки.</response>
        [Produces("application/json")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPut("resend")]
        public async Task<IActionResult> ResendCode(string resendToken, [FromBody]PhoneLoginViewModel phoneModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetUser(phoneModel);
            if (!await _dataProtectorTokenProvider.ValidateAsync("resend_token", resendToken, _userManager, user))
            {
                return BadRequest("Invalid resend token");
            }

            var verifyToken = await _phoneNumberTokenProvider.GenerateAsync("verify_number", _userManager, user);
            var sendResult = await SendRequest(phoneModel.PhoneNumber, verifyToken);
            if (!sendResult)
            {
                return BadRequest("Sending sms failed");
            }

            var newResendToken = await _dataProtectorTokenProvider.GenerateAsync("resend_token", _userManager, user);
            var body = new Dictionary<string, string> { { "resend_token", newResendToken } };

            return Accepted(body);
        }

        /// <summary>
        /// Генерация и отправка кода по sms.
        /// </summary>
        /// <remarks>
        /// В случае, если номер телефона используется впервые, создается новый пользователь.
        /// </remarks>
        /// <returns>Результат отправки sms.</returns>
        /// <response code="202">Отправлено.</response>
        /// <response code="400">Проверьте заполненую форму.</response>
        /// <response code="500">Ошибка отправки.</response>
        /// <param name="phoneModel">Модель телефона пользователя.</param>
        [Produces("application/json")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost("send")]
        public async Task<IActionResult> SendCode([FromBody]PhoneLoginViewModel phoneModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetUser(phoneModel);
            var verifyToken = await _phoneNumberTokenProvider.GenerateAsync("verify_number", _userManager, user);

            var sendResult = await SendRequest(phoneModel.PhoneNumber, verifyToken);
            if (!sendResult)
            {
                return BadRequest("Sending sms failed");
            }

            var resendToken = await _dataProtectorTokenProvider.GenerateAsync("resend_token", _userManager, user);
            var body = new Dictionary<string, string>
            {
                { "resend_token", resendToken }
            };

            return Accepted(body);
        }

        private async Task<User> GetUser(PhoneLoginViewModel loginViewModel)
        {
            var phoneNumber = _userManager.NormalizeKey(loginViewModel.PhoneNumber);
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber)
                ?? new User
                {
                    Id = -1,
                    PhoneNumber = loginViewModel.PhoneNumber,
                    SecurityStamp = loginViewModel.PhoneNumber.Sha256(),
                };

            return user;
        }

        private async Task<bool> SendRequest(string phoneNumber, string verifyToken)
        {
            if (_env.IsDevelopment())
            {
                return await SendSlackRequet(phoneNumber, verifyToken);
            }
            else
            {
                return await SendSmsRequet(phoneNumber, verifyToken);
            }
        }

        private async Task<bool> SendSmsRequet(string phoneNumber, string verifyToken)
        {
            var smsResult = await _smsService.SendAsync(phoneNumber, $"Your login verification code is: {verifyToken}");
            _logger.LogDebug($"verifyToken: {verifyToken} sended by sms");

            return smsResult;
        }

        private async Task<bool> SendSlackRequet(string phoneNumber, string verifyToken)
        {
            var slackResult = await _slackService.SendAsync($"Login verification code is: {verifyToken} for phone: {phoneNumber}");
            _logger.LogDebug($"verifyToken: {verifyToken} sended to slack");

            return slackResult;
        }
    }
}