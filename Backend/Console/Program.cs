using System;
using TransportSystems.Backend.Identity.API;

namespace TransportSystems.Backend.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var identityUsersAPI = IdentityUsersAPIFactory<IIdentityUsersAPI>.Create("http://localhost:82/");

            var inDriverRole = identityUsersAPI.IsInRole(2, "Driver").GetAwaiter().GetResult();

            identityUsersAPI.AsignToRoles(2, new [] { "Driver" }).GetAwaiter().GetResult();

            inDriverRole = identityUsersAPI.IsInRole(2, "Driver").GetAwaiter().GetResult();

            var t = "stop";
        }
    }
}
