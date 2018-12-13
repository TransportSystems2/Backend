using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TransportSystems.Backend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "API";

            var host = CreateWebHostBuilder(args).Build();

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}