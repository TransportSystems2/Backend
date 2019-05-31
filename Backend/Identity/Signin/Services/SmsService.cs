using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TransportSystems.Backend.Identity.Signin.Services
{
    public class SmsService : ISmsService
    {
        private const string SmsEnv = "SMS_TOKEN";
        public SmsService(ILogger<SmsService> logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }

        private ILogger<SmsService> Logger { get; }

        private IConfiguration Configuration { get; }

        public async Task<bool> SendAsync(string phoneNumber, string body)
        {
            var result = false;
            try
            {
                var token = Configuration.GetValue<string>(SmsEnv);
                var client = new HttpClient();
                var query = $"https://sms.ru/sms/send?api_id={token}&to={phoneNumber}&msg={body}&json=1";
                await client.GetAsync(query);

                Logger.LogInformation("Message was sended");
                result = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }

            return result;
        }
    }
}
