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

        private async Task<bool> SendAsync(Dictionary<string, string> dict)
        {
            var result = false;
            try
            {
                var slackToken = Configuration.GetValue<string>(SlackEnv);
                var content = new FormUrlEncodedContent(dict);
                var client = new HttpClient();
                var query = $"https://hooks.slack.com/services/{slackToken}";
                await client.PostAsync(query, content);

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
