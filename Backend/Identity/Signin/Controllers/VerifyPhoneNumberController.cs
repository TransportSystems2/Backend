using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Signin.Models;
using TransportSystems.Backend.Identity.Signin.Services;

namespace TransportSystems.Backend.Identity.Signin.Controllers
{
    [Route("identity/getcode")]
    public class VerifyPhoneNumberController : ControllerBase
    {
        private readonly ISmsService _smsService;
        private readonly DataProtectorTokenProvider<User> _dataProtectorTokenProvider;
        private readonly PhoneNumberTokenProvider<User> _phoneNumberTokenProvider;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<VerifyPhoneNumberController> _logger;

        public VerifyPhoneNumberController(
            ISmsService smsService,
            DataProtectorTokenProvider<User> dataProtectorTokenProvider,
            PhoneNumberTokenProvider<User> phoneNumberTokenProvider,
            UserManager<User> userManager,
            ILogger<VerifyPhoneNumberController> logger)
        {
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _dataProtectorTokenProvider = dataProtectorTokenProvider ?? throw new ArgumentNullException(nameof(dataProtectorTokenProvider));
            _phoneNumberTokenProvider = phoneNumberTokenProvider ?? throw new ArgumentNullException(nameof(phoneNumberTokenProvider));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]PhoneLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetUser(model);
            var (verifyToken, result) = await SendSmsRequet(model, user);

            if (!result)
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

        [HttpPut]
        public async Task<IActionResult> Put(string resendToken, [FromBody]PhoneLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetUser(model);
            if (!await _dataProtectorTokenProvider.ValidateAsync("resend_token", resendToken, _userManager, user))
            {
                return BadRequest("Invalid resend token");
            }

            var (verifyToken, result) = await SendSmsRequet(model, user);

            if (!result)
            {
                return BadRequest("Sending sms failed");
            }

            var newResendToken = await _dataProtectorTokenProvider.GenerateAsync("resend_token", _userManager, user);
            var body = new Dictionary<string, string> { { "resend_token", newResendToken } };

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

        private async Task<(string VerifyToken, bool Result)> SendSmsRequet(PhoneLoginViewModel model, User user)
        {
            var verifyToken = await _phoneNumberTokenProvider.GenerateAsync("verify_number", _userManager, user);
            var result = await _smsService.SendAsync(model.PhoneNumber, $"Your login verification code is: {verifyToken}");
            _logger.LogDebug($"verifyToken: {verifyToken}");

            return (verifyToken, result);
        }
    }
}