using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Infrastructure.Business.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using Xunit;

namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Business.Users
{
    public class TestUser : User
    {

    }

    public class TestUserService<T> : UserService<T>, IUserService<T> where T : User, new()
    {
        public TestUserService(IUserRepository<T> repository, IIdentityUserService identityUserService) : base(repository, identityUserService)
        {
        }

        public override Task<string[]> GetSpecificRoles()
        {
            var specificRoles = new string[] { UserRole.UserRoleName };

            return Task.FromResult(specificRoles);
        }
    }

    public class UserServiceTestSuite<T> where T : TestUser, new()
    {
        public UserServiceTestSuite()
        {
            IdentityUserServiceMock = new Mock<IIdentityUserService>();
            UserRepositoryMock = new Mock<IUserRepository<T>>();
            UserService = new TestUserService<T>(UserRepositoryMock.Object, IdentityUserServiceMock.Object);
        }

        public Mock<IIdentityUserService> IdentityUserServiceMock { get; }

        public Mock<IUserRepository<T>> UserRepositoryMock { get; }

        public IUserService<T> UserService { get; }
    }

    public class UserServiceTests
    {

        public UserServiceTests()
        {
            Suite = new UserServiceTestSuite<TestUser>();
        }

        protected UserServiceTestSuite<TestUser> Suite { get; }

        [Fact]
        public async Task CreateUserWithExistIdentityUser()
        {
            var firstName = "Alexandr";
            var lastName = "Fadeev";
            var phoneNumber = "+77777777";
            var identityUserId = 0;
            var identityUser = new IdentityUser { Id = identityUserId };

            Suite.IdentityUserServiceMock
                .Setup(m => m.GetUserByPhoneNumber(phoneNumber))
                .ReturnsAsync(identityUser);

            Suite.IdentityUserServiceMock
                .Setup(m => m.IsExistById(identityUserId))
                .ReturnsAsync(true);

            Suite.UserRepositoryMock
                .Setup(m => m.IsExistByIdentityUser(identityUserId))
                .ReturnsAsync(false);

            var user = await Suite.UserService.Create(firstName, lastName, phoneNumber);

            Assert.Equal(identityUserId, user.IdentityUserId);

            Suite.IdentityUserServiceMock
                .Verify(m => m.AssignName(identityUserId, firstName, lastName));

            Suite.IdentityUserServiceMock
                .Verify(m =>m.AsignToRoles(identityUserId, It.IsAny<string[]>()));

            Suite.UserRepositoryMock
                .Verify(m => m.Add(It.Is<TestUser>(d => d.IdentityUserId.Equals(identityUserId))));

            Suite.UserRepositoryMock
                .Verify(m => m.Save());
        }

        [Fact]
        public async Task CreateUserWithoutIdentityUser()
        {
            var firstName = "Alexandr";
            var lastName = "Fadeev";
            var phoneNumber = "+77777777";
            var identityUserId = 0;
            var identityUser = new IdentityUser { Id = identityUserId };

            Suite.IdentityUserServiceMock
                .Setup(m => m.GetUserByPhoneNumber(phoneNumber))
                .ReturnsAsync((IdentityUser)null);

            Suite.IdentityUserServiceMock
                .Setup(m => m.Create(firstName, lastName, phoneNumber))
                .ReturnsAsync(identityUser);

            Suite.IdentityUserServiceMock
                .Setup(m => m.IsExistById(identityUserId))
                .ReturnsAsync(true);

            var user = await Suite.UserService.Create(firstName, lastName, phoneNumber);

            Assert.Equal(identityUserId, user.IdentityUserId);

            Suite.IdentityUserServiceMock
                .Verify(m => m.Create(firstName, lastName, phoneNumber));

            Suite.IdentityUserServiceMock
                .Verify(m => m.AssignName(identityUserId, firstName, lastName));

            Suite.IdentityUserServiceMock
                .Verify(m => m.AsignToRoles(identityUserId, It.IsAny<string[]>()));

            Suite.UserRepositoryMock
                .Verify(m => m.Add(It.Is<TestUser>(d => d.IdentityUserId.Equals(identityUserId))));

            Suite.UserRepositoryMock
                .Verify(m => m.Save());
        }

        [Fact]
        public async Task CreateUserWithIdentityCantCreateUser()
        {
            var firstName = "Alexandr";
            var lastName = "Fadeev";
            var phoneNumber = "+77777777";

            Suite.IdentityUserServiceMock
                .Setup(m => m.GetUserByPhoneNumber(phoneNumber))
                .ReturnsAsync((IdentityUser)null);

            Suite.IdentityUserServiceMock
                .Setup(m => m.Create(firstName, lastName, phoneNumber))
                .ReturnsAsync((IdentityUser)null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => Suite.UserService.Create(firstName, lastName, phoneNumber));
        }

        [Fact]
        public async Task CreateDublicateUser()
        {
            var firstName = "Alexandr";
            var lastName = "Fadeev";
            var phoneNumber = "+77777777";
            var identityUser = new IdentityUser();

            Suite.IdentityUserServiceMock
                .Setup(m => m.GetUserByPhoneNumber(phoneNumber))
                .ReturnsAsync(identityUser);

            Suite.UserRepositoryMock
                .Setup(m => m.IsExistByIdentityUser(identityUser.Id))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => Suite.UserService.Create(firstName, lastName, phoneNumber));
        }

        [Fact]
        public async Task GetByIdentityUser()
        {
            var identityUserId = 1;
            var user = new TestUser { IdentityUserId = identityUserId };

            Suite.UserRepositoryMock
                .Setup(m => m.GetByIndentityUser(identityUserId))
                .ReturnsAsync(user);

            var result = await Suite.UserService.GetByIndentityUser(identityUserId);

            Assert.Equal(identityUserId, result.IdentityUserId);
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetByPhoneNumber()
        {
            var phoneNumber = "+77777777";
            var identityUserId = 1;
            var identityUser = new IdentityUser { Id = identityUserId };
            var user = new TestUser { IdentityUserId = identityUserId };

            Suite.IdentityUserServiceMock
                .Setup(m => m.GetUserByPhoneNumber(phoneNumber))
                .ReturnsAsync(identityUser);

            Suite.UserRepositoryMock
                .Setup(m => m.GetByIndentityUser(identityUserId))
                .ReturnsAsync(user);

            var result = await Suite.UserService.GetByPhoneNumber(phoneNumber);

            Assert.Equal(identityUserId, result.IdentityUserId);
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task IsExistByIdentityUser()
        {
            var identityUserId = 1;

            Suite.UserRepositoryMock
                .Setup(m => m.IsExistByIdentityUser(identityUserId))
                .ReturnsAsync(true);

            var result = await Suite.UserService.IsExistByIdentityUser(identityUserId);

            Assert.True(result);
        }
    }
}
