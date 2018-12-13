using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public abstract class UserService<T> : DomainService<T>, IUserService<T> where T : User, new()
    {
        public UserService(IUserRepository<T> repository, IIdentityUserService identityUserService)
            : base(repository)
        {
            IdentityUserService = identityUserService;
        }

        protected new IUserRepository<T> Repository => (IUserRepository<T>)base.Repository;

        public IIdentityUserService IdentityUserService { get; }

        public abstract string[] GetSpecificRoles();

        public async Task<T> Create(string firstName, string lastName, string phoneNumber)
        {
            var identityUser = await IdentityUserService.GetUserByPhoneNumber(phoneNumber);

            if (identityUser == null)
            {
                identityUser = await IdentityUserService.Create(firstName, lastName, phoneNumber);
            }

            if ((firstName != null) && (lastName != null))
            {
                await IdentityUserService.AssignName(identityUser.Id, firstName, lastName);
            }

            var specificRoles = GetSpecificRoles();
            await IdentityUserService.AsignToRoles(identityUser.Id, specificRoles);

            var result = new T
            {
                IdentityUserId = identityUser.Id,
            };

            await Repository.Add(result);
            await Repository.Save();

            return result;
        }

        public Task<T> GetByIndentityUser(int userIdentity)
        {
            return Repository.GetByIndentityUser(userIdentity);
        }

        public async Task<T> GetByPhoneNumber(string phoneNumber)
        {
            T result = null;

            var identityUser = await IdentityUserService.GetUserByPhoneNumber(phoneNumber);
            if (identityUser != null)
            {
                result = await GetByIndentityUser(identityUser.Id);
            }

            return result;
        }

        public Task<bool> IsExistByIdentityUser(int userIdnentity)
        {
            return Repository.IsExistByIdentityUser(userIdnentity);
        }

        protected override Task<bool> DoVerifyEntity(T entity)
        {
            throw new NotImplementedException();
        }
    }
}