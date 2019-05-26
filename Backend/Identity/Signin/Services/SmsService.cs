using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TransportSystems.Backend.Identity.Signin.Services
{
    public class SmsService : ISmsService
    {
        public SmsService(ILogger<SmsService> logger, IConfiguration configuration)
        {
            Logger = logger;
            ApiId = configuration["SMS_TOKEN"];
        }

        private string ApiId { get; }

        private ILogger<SmsService> Logger { get; }

        public async Task<bool> SendAsync(string phoneNumber, string body)
        {
            var result = false;
            try
            {
                var client = new HttpClient();
                var query = $"https://sms.ru/sms/send?api_id={ApiId}&to={phoneNumber}&msg={body}&json=1";
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
