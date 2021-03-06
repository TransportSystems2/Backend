﻿using System;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using Xunit;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Tests.Users
{
    public class TestUser : IdentityUser
    {
    }

    public class TestUserService :
        IdentityUserService<TestUser>
    {
        public TestUserService(IIdentityUserRepository<TestUser> repository)
            : base(repository)
        {
        }

        public override string GetDefaultRole()
        {
            return "TestRole";
        }
    }

    public class UserServiceTestSuite
    {
        public UserServiceTestSuite()
        {
            UserRepositoryMock = new Mock<IIdentityUserRepository<TestUser>>();
            ServiceMock = new Mock<TestUserService>(
                UserRepositoryMock.Object);
            ServiceMock.CallBase = true;
        }

        public Mock<IIdentityUserRepository<TestUser>> UserRepositoryMock { get; }

        public Mock<TestUserService> ServiceMock { get; }

        public TestUserService Service => ServiceMock.Object;
    }

    public class UserServiceTests
    {

        public UserServiceTests()
        {
            Suite = new UserServiceTestSuite();
        }

        protected UserServiceTestSuite Suite { get; }

        [Fact]
        public async Task Create()
        {
            var phoneNumber = "79998887766";

            Suite.UserRepositoryMock
                .Setup(m => m.IsExistByPhoneNumber(phoneNumber))
                .ReturnsAsync(false);

            var result = await Suite.Service.Create(phoneNumber);

            Suite.UserRepositoryMock
                .Verify(m => m.Add(It.Is<TestUser>(
                    u => u.PhoneNumber.Equals(phoneNumber))));

            Suite.UserRepositoryMock
                .Verify(m => m.Save());

            Suite.UserRepositoryMock
                .Verify(m => m.AsignToRoles(It.IsAny<int>(), It.Is<string[]>(
                    roles => Array.Exists(roles, i => i.Equals(Suite.Service.GetDefaultRole()))
                )));
        }

        [Fact]
        public async Task CreateWhenUserExist()
        {
            var phoneNumber = "79998887766";

            Suite.UserRepositoryMock
                .Setup(m => m.IsExistByPhoneNumber(phoneNumber))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => Suite.Service.Create(phoneNumber));
        }

        [Fact]
        public async Task GetById()
        {
            var user = new TestUser
            {
                Id = 1
            };

            Suite.UserRepositoryMock
                .Setup(m => m.Get(user.Id))
                .ReturnsAsync(user);

            var result = await Suite.Service.Get(user.Id);

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetByPhoneNumber()
        {
            var user = new TestUser
            {
                PhoneNumber = "79998887766"
            };

            Suite.UserRepositoryMock
                .Setup(m => m.GetByPhoneNumber(user.PhoneNumber))
                .ReturnsAsync(user);

            var result = await Suite.Service.GetByPhoneNumber(user.PhoneNumber);

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task Exist()
        {
            var userId = 1;

            Suite.UserRepositoryMock
                .Setup(m => m.IsExist(userId))
                .ReturnsAsync(true);

            var result = await Suite.Service.IsExist(userId);

            Assert.True(result);
        }
    }
}
