using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Core.Database
{
    public static class DbUserInitializator
    {
        private const string DefaultPassword = "passworD1~";
        public static void Initialize(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var services = scope.ServiceProvider;
                services.GetService<IdentityContext>().Database.Migrate();
                var roleService = services.GetRequiredService<IRoleService>();
                var userService = services.GetRequiredService<IUserService>();
                InitRoles(roleService).GetAwaiter().GetResult();
                InitUsers(userService, roleService).GetAwaiter().GetResult();
            }
        }

        private static async Task InitRoles(IRoleService roleService)
        {
            if (await roleService.FindByNameAsync(UserRole.AdminRoleName) == null)
            {
                await roleService.CreateAsync(new UserRole(UserRole.AdminRoleName));
            }

            if (await roleService.FindByNameAsync(UserRole.ModeratorRoleName) == null)
            {
                await roleService.CreateAsync(new UserRole(UserRole.ModeratorRoleName));
            }

            if (await roleService.FindByNameAsync(UserRole.DispatcherRoleName) == null)
            {
                await roleService.CreateAsync(new UserRole(UserRole.DispatcherRoleName));
            }

            if (await roleService.FindByNameAsync(UserRole.DriverRoleName) == null)
            {
                await roleService.CreateAsync(new UserRole(UserRole.DriverRoleName));
            }

            if (await roleService.FindByNameAsync(UserRole.CustomerRoleName) == null)
            {
                await roleService.CreateAsync(new UserRole(UserRole.CustomerRoleName));
            }
        }

        private static async Task InitUsers(IUserService userService, IRoleService roleService)
        {
            var roles = new List<string> { UserRole.AdminRoleName };
            await CreateUserIfNotExists(userService, roleService, "admin", "Admin", "Adminovich", roles);

            roles = new List<string> { UserRole.ModeratorRoleName };
            await CreateUserIfNotExists(userService, roleService, "moderator1", "Moderator1", "Moderatorovich1", roles);
            await CreateUserIfNotExists(userService, roleService, "moderator2", "Moderator2", "Moderatorovich2", roles);
            await CreateUserIfNotExists(userService, roleService, "moderator3", "Moderator2", "Moderatorovich2", roles);

            roles = new List<string> { UserRole.DispatcherRoleName };
            await CreateUserIfNotExists(userService, roleService, "dispatcher1", "Dispatcher1", "Dispatcherivich1", roles);
            await CreateUserIfNotExists(userService, roleService, "dispatcher2", "Dispatcher2", "Dispatcherivich2", roles);
            await CreateUserIfNotExists(userService, roleService, "dispatcher3", "Dispatcher3", "Dispatcherivich3", roles);

            roles = new List<string> { UserRole.DriverRoleName };
            await CreateUserIfNotExists(userService, roleService, "driver1", "Driver1", "Driverovich1", roles);
            await CreateUserIfNotExists(userService, roleService, "driver2", "Driver2", "Driverovich2", roles);
            await CreateUserIfNotExists(userService, roleService, "driver3", "Driver3", "Driverovich3", roles);

            roles = new List<string> { UserRole.CustomerRoleName };
            await CreateUserIfNotExists(userService, roleService, "customer1", "Customer1", "Customerovich1", roles);
            await CreateUserIfNotExists(userService, roleService, "customer2", "Customer2", "Customerovich2", roles);
            await CreateUserIfNotExists(userService, roleService, "customer3", "Customer3", "Customerovich3", roles);
        }

        private static async Task<bool> CreateUserIfNotExists(IUserService userService, IRoleService roleService, string name, string firstName, string lastName, List<string> roles)
        {
            var user = await userService.FindByNameAsync(name);
            if (user != null)
            {
                return true;
            }

            user = new User
            {
                UserName = name,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userService.CreateAsync(user, DefaultPassword);

            var addedRolesResult = await AsignRolesToUser(userService, roleService, user, roles);
            return result.Succeeded;
        }

        private static async Task<IdentityResult> AsignRolesToUser(IUserService userService, IRoleService roleService, User user, List<string> roles)
        {
            var userRoles = await userService.GetRolesAsync(user);
            var allRoles = roleService.Roles.ToList();
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            var result = await userService.AddToRolesAsync(user, addedRoles);
            if (result.Succeeded)
            {
                result = await userService.RemoveFromRolesAsync(user, removedRoles);
            }

            return result;
        }
    }
}
