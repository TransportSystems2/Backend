using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business
{
    public abstract class IdentityUserService<T> :
        DomainService<T>,
        IIdentityUserService<T>
        where T : IdentityUser, new()
    {
        public IdentityUserService(IIdentityUserRepository<T> repository)
            : base(repository)
        {
        }

        protected new IIdentityUserRepository<T> Repository => (IIdentityUserRepository<T>)base.Repository;

        public abstract string GetDefaultRole();

        public async Task<T> Create(string phoneNumber)
        {
            if (!IsValidPhoneNumber(phoneNumber))
            {
                throw new ArgumentException($"Invalid phoneNumber={phoneNumber}", nameof(phoneNumber));
            }

            if (await IsExistByPhoneNumber(phoneNumber))
            {
                throw new EntityAlreadyExistsException($"User with phoneNumber={phoneNumber} already exists"
                , "PhoneNumber");
            }

            var result = new T
            {
                PhoneNumber = phoneNumber,
            };

            await Repository.Add(result);
            await Repository.Save();

            await AsignToRoles(result.Id, new[] { GetDefaultRole() });

            return result;
        }

        public virtual async Task<bool> IsNeedAssignName(int id)
        {
            var user = await Get(id);
            if (user == null)
            {
                throw new ArgumentException($"Id:{id} is null", "Id");
            }

            return string.IsNullOrEmpty(user.FirstName);
        }

        public async Task<T> AsignName(int id, string firstName, string lastName)
        {
            var user = await Get(id);
            if (user == null)
            {
                throw new ArgumentException($"Id:{id} is null", "Id");
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException("FirstName is null", "FirstName");
            }

            user.FirstName = firstName;
            user.LastName = lastName;

            await Repository.Update(user);
            await Repository.Save();

            return user;
        }

        public async Task<bool> IsExistByPhoneNumber(string phoneNumber)
        {
            return await Repository.IsExistByPhoneNumber(phoneNumber);
        }

        public Task<bool> IsInRole(int id, string role)
        {
            return Repository.IsInRole(id, role);
        }

        public Task<bool> IsUndefined(int id)
        {
            return Repository.IsUndefined(id);
        }

        public async Task AsignToRoles(int id, string[] roles)
        {
            await Repository.AsignToRoles(id, roles);
        }

        public Task<T> GetByPhoneNumber(string phoneNumber)
        {
            return Repository.GetByPhoneNumber(phoneNumber);
        }

        protected bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.Match(phoneNumber, @"^(7[0-9]{10})$").Success;
        }

        protected override Task<bool> DoVerifyEntity(T entity)
        {
            return Task.FromResult(true);
        }
    }
}