using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business
{
    public class IdentityUserService : IIdentityUserService
    {
        protected IIdentityUserRepository Repository { get; set; }

        public IdentityUserService(IIdentityUserRepository repository)
        {
            Repository = repository;
        }

        public async Task<IdentityUser> Create(string firstName, string lastName, string phoneNumber)
        {
            if (!IsValidPhoneNumber(phoneNumber))
            {
                throw new ArgumentException($"Invalid phoneNumber={phoneNumber}", nameof(phoneNumber));
            }

            var existByPhoneNumber = await IsExistByPhoneNumber(phoneNumber);
            if (existByPhoneNumber)
            {
                throw new EntityAlreadyExistsException($"User with phoneNumber={phoneNumber} already exists", "PhoneNumber");
            }

            var result = new IdentityUser
            {
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName
            };

            await Repository.Add(result);

            return result;
        }

        public async Task<bool> IsExistById(int id)
        {
            return await Repository.IsExist(id);
        }

        public async Task<bool> IsExistByPhoneNumber(string phoneNumber)
        {
            return await Repository.IsExistByPhoneNumber(phoneNumber);
        }

        public Task<bool> IsInRole(int userId, string role)
        {
            return Repository.IsInRole(userId, role);
        }

        public Task<bool> IsUndefined(int userId)
        {
            return Repository.IsUndefined(userId);
        }

        public async Task AsignToRoles(int id, string[] roles)
        {
            await Repository.AsignToRoles(id, roles);
        }

        public async Task AssignName(int id, string firstName, string lastName)
        {
            var user = await Repository.Get(id);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with id = {id}, doesn't exist", "Id");
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException("FirstName is null", "FirstName");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException("LastName is null", "LastName");
            }

            await Repository.Update(user);
            await Repository.Save();
        }

        public Task<IdentityUser> GetUserByPhoneNumber(string phoneNumber)
        {
            return Repository.GetByPhoneNumber(phoneNumber);
        }

        public Task<ICollection<IdentityUser>> GetUsers()
        {
            return Repository.GetAll();
        }

        public Task<IdentityUser> GetUser(int id)
        {
            return Repository.Get(id);
        }

        public async Task DeleteUser(int id)
        {
            IdentityUser user = await GetUser(id);
            await Repository.Remove(user);
        }

        protected bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.Match(phoneNumber, @"^(7[0-9]{10})$").Success;
        }
    }
}