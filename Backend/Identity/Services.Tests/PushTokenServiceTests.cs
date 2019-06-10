using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Business;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain.Exceptions;
using TransportSystems.Backend.Identity.Core.Domain.Interfaces;
using TransportSystems.Backend.Identity.Core.Interfaces;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Routing
{
    public class PushTokenServiceTestSuite
    {
        public PushTokenServiceTestSuite()
        {
            PushTokenRepositoryMock = new Mock<IPushTokenRepository>();
            UserServiceMock = new Mock<IUserService>();
            PushTokenService = new PushTokenService(PushTokenRepositoryMock.Object, UserServiceMock.Object);
        }

        public Mock<IPushTokenRepository> PushTokenRepositoryMock { get; }

        public Mock<IUserService> UserServiceMock { get; }

        public IPushTokenService PushTokenService { get; }
    }

    public class PushTokenServiceTests
    {
        public PushTokenServiceTests()
        {
            Suite = new PushTokenServiceTestSuite();
        }

        protected PushTokenServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreateToken()
        {
            var value = "token";
            var type = PushTokenType.Android;
            var userId = 1;

            Suite.UserServiceMock
                .Setup(m => m.ExistAsync(userId))
                .ReturnsAsync(true);

            var result = await Suite.PushTokenService.CreateToken(value, type, userId);

            Suite.PushTokenRepositoryMock
            .Verify(
                m => m.Add(
                    It.Is<PushToken>(
                        e => e.Value.Equals(value) &&
                        e.Type.Equals(type) &&
                        e.UserId.Equals(userId))),
                Times.Once);

                Suite.PushTokenRepositoryMock
                .Verify(m => m.Save(), Times.Once);

            Assert.Equal(value, result.Value);
            Assert.Equal(type, result.Type);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task ValidateEmptyToken()
        {
           await  Assert.ThrowsAsync<ArgumentNullException>("Value", () => Suite.PushTokenService.CreateToken(string.Empty, PushTokenType.Android, 0));
        }

        [Fact]
        public async Task ValidateNotExistUser()
        {
            var userId = 0;
            
            Suite.UserServiceMock
                .Setup(m => m.ExistAsync(userId))
                .ReturnsAsync(false);

           await  Assert.ThrowsAsync<EntityNotFoundException>(() => Suite.PushTokenService.CreateToken("token", PushTokenType.Android, 0));
        }

        [Fact]
        public async Task DublicateToken()
        {
            var value = "token";

            Suite.PushTokenRepositoryMock
                .Setup(m => m.ByValue(value))
                .ReturnsAsync(new PushToken());

            await Assert.ThrowsAsync<DublicateException>(() => Suite.PushTokenService.CreateToken(value, PushTokenType.Android, 0));
        }

        [Fact]
        public async Task ExistToken()
        {
            var value = "token";

            Suite.PushTokenRepositoryMock
            .Setup(m => m.ByValue(value))
            .ReturnsAsync(new PushToken());

            var result = await Suite.PushTokenService.ExistToken(value);

            Assert.True(result);
        }

        [Fact]
        public async Task ReadTokensForOneUser()
        {
            var userId = 1;

            var token1 = new PushToken { Value = "token1", Type = PushTokenType.Android, UserId = userId };
            var token2 = new PushToken { Value = "token2", Type = PushTokenType.iOS, UserId = userId };

            var tokens = new List<PushToken> { token1, token2 };

            Suite.PushTokenRepositoryMock
            .Setup(m => m.Get(new int [] { userId }))
            .ReturnsAsync(tokens);

            var result = await Suite.PushTokenService.ReadTokens(userId);

            Assert.Equal(2, result.Count);
            Assert.All(result, e => e.UserId.Equals(userId));
            Assert.Contains(token1, result);
            Assert.Contains(token2, result);
        }

        [Fact]
        public async Task ReadTokensForFewUser()
        {
            var usersId = new int[] {1, 2};

            var token1 = new PushToken { Value = "token1", Type = PushTokenType.Android, UserId = usersId[0] };
            var token2 = new PushToken { Value = "token2", Type = PushTokenType.iOS, UserId = usersId[0] };
            var token3 = new PushToken { Value = "token3", Type = PushTokenType.iOS, UserId = usersId[1] };

            var tokens = new List<PushToken> { token1, token2, token3 };

            Suite.PushTokenRepositoryMock
            .Setup(m => m.Get(usersId))
            .ReturnsAsync(tokens);

            var result = await Suite.PushTokenService.ReadTokens(usersId);

            Assert.Equal(3, result.Count);
            Assert.Contains(token1, result);
            Assert.Contains(token2, result);
            Assert.Contains(token3, result);
        }

        [Fact]
        public async Task DeleteToken()
        {
            var value = "token";
            var userId = 1;
            var tokenType = PushTokenType.Android;

            var token = new PushToken
            {
                Value = value,
                Type = tokenType,
                UserId = userId
            };

            Suite.PushTokenRepositoryMock
                .Setup(m => m.ByValue(value))
                .ReturnsAsync(token);

            var result = await Suite.PushTokenService.DeleteToken(value, tokenType, userId);

            Suite.PushTokenRepositoryMock
                .Setup(
                    m => m.Remove(
                        It.Is<PushToken>(
                            e => e.Value.Equals(value) &&
                            e.Type.Equals(tokenType) &&
                            e.UserId.Equals(userId))));

            Suite.PushTokenRepositoryMock.Verify(m => m.Save(), Times.Once);

            Assert.Equal(value, token.Value);
            Assert.Equal(tokenType, token.Type);
            Assert.Equal(userId, token.UserId);
        }

        [Fact]
        public async Task DeleteWithEmptyToken()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => Suite.PushTokenService.DeleteToken(
                        string.Empty,
                        PushTokenType.Android,
                        1));
        }

        [Fact]
        public async Task DeleteNotExistToken()
        {
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () =>Suite.PushTokenService.DeleteToken(
                    "token",
                    PushTokenType.Android,
                    1));
        }

        [Fact]
        public async Task DeleteDifferentTokenType()
        {
            var value = "token";
            var tokenType = PushTokenType.Android;

            var token = new PushToken
            {
                Value = value,
                Type = tokenType
            };

            Suite.PushTokenRepositoryMock
            .Setup(m => m.ByValue(value))
            .ReturnsAsync(token);

            await Assert.ThrowsAsync<ArgumentException>(
                () =>Suite.PushTokenService.DeleteToken(
                    value,
                    PushTokenType.iOS,
                    1));
        }

        [Fact]
        public async Task DeleteDifferentUserId()
        {
            var value = "token";
            var tokenType = PushTokenType.Android;
            var userId = 0;

            var token = new PushToken
            {
                Value = value,
                Type = tokenType,
                UserId = userId
            };

            Suite.PushTokenRepositoryMock
            .Setup(m => m.ByValue(value))
            .ReturnsAsync(token);

            await Assert.ThrowsAsync<ArgumentException>(
                () =>Suite.PushTokenService.DeleteToken(
                    value,
                    tokenType,
                    1));
        }

        [Fact]
        public async Task DeleteTokensByUserId()
        {
            var userId = 1;

            var token1 = new PushToken { Value = "token1", Type = PushTokenType.Android, UserId = userId };
            var token2 = new PushToken { Value = "token2", Type = PushTokenType.iOS, UserId = userId };
            var token3 = new PushToken { Value = "token3", Type = PushTokenType.iOS, UserId = userId };

            var tokens = new List<PushToken> { token1, token2, token3 };

            Suite.PushTokenRepositoryMock
                .Setup(m => m.Get(new int [] { userId }))
                .ReturnsAsync(tokens);

                Suite.PushTokenRepositoryMock
                    .Setup(m => m.ByValue("token1"))
                    .ReturnsAsync(token1);

                Suite.PushTokenRepositoryMock
                    .Setup(m => m.ByValue("token2"))
                    .ReturnsAsync(token2);

                Suite.PushTokenRepositoryMock
                    .Setup(m => m.ByValue("token3"))
                    .ReturnsAsync(token3);

            await Suite.PushTokenService.DeleteTokensByUser(userId);

            Suite.PushTokenRepositoryMock.Verify(m => m.Get(new int [] { userId }), Times.Once);
            Suite.PushTokenRepositoryMock.Verify(m => m.Remove(token1), Times.Once);
            Suite.PushTokenRepositoryMock.Verify(m => m.Remove(token2), Times.Once);
            Suite.PushTokenRepositoryMock.Verify(m => m.Remove(token3), Times.Once);
        }
    }
}