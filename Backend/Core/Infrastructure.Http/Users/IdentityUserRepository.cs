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
    public class IdentityUserRepository<T> : IIdentityUserRepository<T> where T : IdentityUser
    {
        public IdentityUserRepository(IIdentityUsersAPI identityUsersAPI)
        {
            IdentityUsersAPI = identityUsersAPI;
        }

        public Task<ICollection<T>> AllUsers => GetAllUsers();

        protected IIdentityUsersAPI IdentityUsersAPI { get; }

        public async Task<T> Add(T entity)
        {
            var userModel = Mapper.Map<UserModel>(entity);
            var createdUserModel = await IdentityUsersAPI.Create(userModel);

            entity.Id = createdUserModel.Id;
            
            return Mapper.Map<T>(createdUserModel);
        }

        Task IRepository<T>.Add(T entity)
        {
            return Add(entity);
        }

        public async Task AddRange(params T[] entities)
        {
            foreach(var entity in entities)
            {
                await Add(entity);
            }
        }

        public Task AddRange(ICollection<T> entities)
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

        public async Task<T> Get(int id)
        {
            var userModel = await IdentityUsersAPI.ByIdAsync(id);

            return Mapper.Map<T>(userModel);
        }

        public async Task<ICollection<T>> Get(int[] idArray)
        {
            var usersModel = await IdentityUsersAPI.ByIdsAsync(idArray);

            return Mapper.Map<IEnumerable<UserModel>, ICollection<T>>(usersModel);
        }

        public async Task<ICollection<T>> GetAll()
        {
            var usersModel = await IdentityUsersAPI.GetAllAsync();

            return Mapper.Map<IEnumerable<UserModel>, ICollection<T>>(usersModel);
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

        public async Task<T> GetByPhoneNumber(string phoneNumber)
        {
            T result;

            try
            {
                var userModel = await IdentityUsersAPI.FindByPhoneNumberAsync(phoneNumber);
                result = Mapper.Map<T>(userModel);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public Task Remove(T entity)
        {
            throw new NotImplementedException();
        }

        public Task Save()
        {
            return Task.CompletedTask;
        }

        public Task Update(T entity)
        {
            var userModel = Mapper.Map<UserModel>(entity);

            return IdentityUsersAPI.Update(entity.Id, userModel);
        }

        private async Task<ICollection<T>> GetAllUsers()
        {
            var usersModel = await IdentityUsersAPI.GetAllAsync();

            return Mapper.Map<IEnumerable<UserModel>, ICollection<T>>(usersModel);
        }
    }
}