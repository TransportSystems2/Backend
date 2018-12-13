using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain.Exceptions;
using TransportSystems.Backend.Identity.Core.Domain.Interfaces;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Core.Business
{
    public class PushTokenService : Service<PushToken, IPushTokenRepository>, IPushTokenService
    {
        public PushTokenService(IPushTokenRepository repository, IUserService userService)
            : base(repository)
        {
            UserService = userService;
        }

        protected IUserService UserService { get; }

        public async Task<PushToken> CreateToken(string value, PushTokenType type, int userId)
        {
            if (await ExistToken(value))
            {
                 throw new DublicateException($"token with same value alredy exist. Value: {value}");
            }

            var token = new PushToken
            {
                Value = value,
                Type = type,
                UserId = userId
            };

            return await Create(token);
        }

        public async Task<bool> ExistToken(string value)
        {
            var token = await Repository.ByValue(value);

            return token != null;
        }

        public Task<ICollection<PushToken>> ReadTokens(int userId)
        {
            return ReadTokens(new int[] { userId });
        }

        public Task<ICollection<PushToken>> ReadTokens(int[] usersId)
        {
            return Repository.Get(usersId);
        }

        public async Task<PushToken> DeleteToken(string value, PushTokenType type, int userId)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Value");
            }

            var token = await Repository.ByValue(value);
            if (token == null)
            {
                throw new EntityNotFoundException($"not found token with value: {value}");
            }

            if (!token.Type.Equals(type))
            {
                throw new ArgumentException(
                    $"token has different type." +
                    $"Expected: {type}.\n" +
                    $"Actual: {token.Type}");
            }

            if (!token.UserId.Equals(userId))
            {
                throw new ArgumentException(
                    $"token has different user." +
                    $"Expected: {userId}.\n" +
                    $"Actual: {token.UserId}");
            }

            return await Delete(token);
        }

        public async Task DeleteTokensByUser(int userId)
        {
            var tokens = await ReadTokens(userId);

            foreach (var token in tokens)
            {
                await DeleteToken(token.Value, token.Type, token.UserId);
            }
        }

        protected async override Task<bool> ValidateEntity(PushToken entity)
        {
            if (string.IsNullOrEmpty(entity.Value))
            {
                throw new ArgumentNullException("Value");
            }

            if (!await UserService.ExistAsync(entity.UserId))
            {
                throw new EntityNotFoundException($"Not found user with id: {entity.UserId}");
            }

            return true;
        }
    }
}