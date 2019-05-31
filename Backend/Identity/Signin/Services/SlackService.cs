using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TransportSystems.Backend.Identity.Signin.Services
{
    public class SlackService : ISlackService
    {
        private const string SlackEnv = "SLACK";

        private const string UserName = "identity";
        public SlackService(ILogger<SlackService> logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }

        private ILogger<SlackService> Logger { get; }

        private IConfiguration Configuration { get; }

        public Task<bool> SendAsync(string message)
        {
            var parameters = new Dictionary<string, string>();
            var payloadParam = new { text = message, username = UserName };
            string serializedPayloadParam = JsonConvert.SerializeObject(payloadParam);
            parameters.Add("payload", serializedPayloadParam);

            return SendAsync(parameters);
        }

        private async Task<bool> SendAsync(Dictionary<string, string> parameters)
        {
            var slackUri = Configuration.GetValue<string>(SlackEnv);
            if (string.IsNullOrEmpty(slackUri))
            {
                Logger.LogWarning("slack service can't send the code. Check SLACK env." +
                    "For more information https://github.com/TransportSystems2/Backend/wiki/Переменные-окружения");
                    
                return false;
            }

            var result = false;
            try
            {
                var content = new FormUrlEncodedContent(parameters);
                var client = new HttpClient();
                await client.PostAsync(slackUri, content);

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
