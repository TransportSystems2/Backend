using IdentityModel.Client;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using TransportSystems.Backend.Identity.API;

namespace TransportSystems.Backend.Identity.Manage.Controllers.Test
{
    [Route("identity/[controller]")]
    public class TestController : Controller
    {
        public TestController()
        {
            var client = GetClient();
            IdentityUsersAPI = IdentityUsersAPIFactory<IIdentityUsersAPI>.Create(client);
        }

        public IIdentityUsersAPI IdentityUsersAPI { get; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await IdentityUsersAPI.GetAllAsync();

            return Ok(users);
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5003/")
            };

            var token = GetTokenAsync().GetAwaiter().GetResult();
            client.SetBearerToken(token);

            return client;
        }

        private async Task<string> GetTokenAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5002/");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                throw new EntryPointNotFoundException();
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, "TSAPI", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("TSAPI");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                throw new EntryPointNotFoundException();
            }

            return tokenResponse.AccessToken;
        }
    }
}