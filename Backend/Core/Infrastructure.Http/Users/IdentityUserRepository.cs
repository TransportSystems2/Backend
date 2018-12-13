using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Identity.API;
using TransportSystems.Backend.Identity.Core.Data.External.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Http.Users
{
    public class IdentityUserRepository : IIdentityUserRepository
    {
        public IdentityUserRepository(IIdentityUsersAPI identityUsersAPI)
        {
            IdentityUsersAPI = identityUsersAPI;
        }

        public Task<ICollection<IdentityUser>> AllUsers => GetAllUsers();

        protected IIdentityUsersAPI IdentityUsersAPI { get; }

        public async Task Add(IdentityUser entity)
        {
            var userModel = Mapper.Map<UserModel>(entity);
            var createUserModel = await IdentityUsersAPI.Create(userModel);
            entity.Id = createUserModel.Id;
        }

        public async Task AddRange(params IdentityUser[] entities)
        {
            foreach(var entity in entities)
            {
                await Add(entity);
            }
        }

        public Task AddRange(ICollection<IdentityUser> entities)
        {
            return AddRange(entities.ToArray());
        }

        public Task<bool> IsExist(int id)
        {
            return IdentityUsersAPI.IsExistById(id);
        }

        public Task<bool> IsExistByPhoneNumber(string phoneNumber)
        {
            return IdentityUsersAPI.IsExistByPhoneNumber(phoneNumber);
        }

        public Task<bool> IsInRole(int userId, string role)
        {
            return IdentityUsersAPI.IsInRole(userId, role);
        }

        public Task<bool> IsUndefined(int userId)
        {
            return IdentityUsersAPI.IsUndefined(userId);
        }

        public async Task<IdentityUser> Get(int id)
        {
            var userModel = await IdentityUsersAPI.ByIdAsync(id);

            return Mapper.Map<IdentityUser>(userModel);
        }

        public async Task<ICollection<IdentityUser>> Get(int[] idArray)
        {
            var usersModel = await IdentityUsersAPI.ByIdsAsync(idArray);

            return Mapper.Map<IEnumerable<UserModel>, ICollection<IdentityUser>>(usersModel);
        }

        public async Task<ICollection<IdentityUser>> GetAll()
        {
            var usersModel = await IdentityUsersAPI.GetAllAsync();

            return Mapper.Map<IEnumerable<UserModel>, ICollection<IdentityUser>>(usersModel);
        }

        public async Task<bool> AsignToRoles(int userId, string[] roles)
        {
            try
            {
                await IdentityUsersAPI.AsignToRoles(userId, roles);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IdentityUser> GetByPhoneNumber(string phoneNumber)
        {
            IdentityUser result;

            try
            {
                var userModel = await IdentityUsersAPI.FindByPhoneNumberAsync(phoneNumber);
                result = Mapper.Map<IdentityUser>(userModel);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public Task Remove(IdentityUser entity)
        {
            throw new NotImplementedException();
        }

        public Task Save()
        {
            return Task.CompletedTask;
        }

        public Task Update(IdentityUser entity)
        {
            var userModel = Mapper.Map<UserModel>(entity);

            return IdentityUsersAPI.Update(entity.Id, userModel);
        }

        private async Task<ICollection<IdentityUser>> GetAllUsers()
        {
            var usersModel = await IdentityUsersAPI.GetAllAsync();

            return Mapper.Map<IEnumerable<UserModel>, ICollection<IdentityUser>>(usersModel);
        }

        public Task<ITransaction> BeginTransaction()
        {
            throw new NotImplementedException();
        }
    }
}