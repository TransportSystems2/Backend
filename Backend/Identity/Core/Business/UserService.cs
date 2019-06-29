using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Core.Business
{
    public class UserService : UserManager<User>, IUserService
    {
        public UserService(
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger)
            : base(
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            var user = await FindByIdAsync(id);
            if (user == null)
            {
                var error = new IdentityError
                {
                    Code = "404",
                    Description = $"User not found id: {id}"
                };

                return IdentityResult.Failed(error);
            }

            return await base.DeleteAsync(user);
        }

        public async Task<bool> ExistAsync(int id)
        {
            var user = await FindByIdAsync(id);

            return user != null;
        }

        public async Task<bool> IsInRoleAsync(int id, string role)
        {
            var user = await FindByIdAsync(id);

            return await IsInRoleAsync(user, role);
        }

        public async Task<User> FindByIdAsync(int id)
        {
            return await base.FindByIdAsync(id.ToString());
        }

        public IEnumerable<User> FindByIds(IEnumerable<int> ids)
        {
            return Users.Where(usr => ids.Contains(usr.Id));
        }

        public User FindByPhoneNumber(string phoneNumber)
        {
            return Users.SingleOrDefault(usr => usr.PhoneNumber.Equals(phoneNumber));
        }

        public async Task<ICollection<User>> GetUsersByCompanyInRoleAsync(int companyId, string role)
        {
            var allCompanyUsers = Users.Where(u => u.CompanyId.Equals(companyId));

            var result = new List<User>();
            foreach (var user in allCompanyUsers)
            {
                if (await IsInRoleAsync(user.Id, role))
                {
                    result.Add(user);
                }
            }

            return result;
        }
    }
}