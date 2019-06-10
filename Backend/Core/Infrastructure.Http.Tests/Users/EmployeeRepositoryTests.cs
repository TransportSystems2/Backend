using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Infrastructure.Http.Mapping;
using TransportSystems.Backend.Core.Infrastructure.Http.Users;
using TransportSystems.Backend.Identity.API;
using TransportSystems.Backend.Identity.Core.Data.External.Users;
using Xunit;

namespace Infrastructure.Http.Tests.Users
{
    public class TestEmployee : Employee
    {
    }
    
    public class EmployeeRepositoryTestSuite
    {
        public EmployeeRepositoryTestSuite()
        {
            MappingService = new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new UsersProfile());
                    cfg.CreateMap<UserModel, TestEmployee>();
                }).CreateMapper();

            APIMock = new Mock<IIdentityUsersAPI>();
            RepositoryMock = new Mock<EmployeeRepository<TestEmployee>>(APIMock.Object, MappingService);
            RepositoryMock.CallBase = true;
        }

        public IMapper MappingService { get; }

        public Mock<IIdentityUsersAPI> APIMock { get; }

        public Mock<EmployeeRepository<TestEmployee>> RepositoryMock { get; }

        public EmployeeRepository<TestEmployee> Repository => RepositoryMock.Object;
    }

    public class EmployeeRepositoryTests
    {
        public EmployeeRepositoryTests()
        {
            Suite = new EmployeeRepositoryTestSuite();
        }

        protected EmployeeRepositoryTestSuite Suite { get; }

        [Fact]
        public async Task GetByCompanyResultEmployeersCountEqualUserModelCount()
        {
            var commonId = 1;
            var companyId = commonId++;
            var role = "employee";

            var users = new List<UserModel>
            {
                new UserModel(),
                new UserModel()
            };

            Suite.APIMock
                .Setup(m => m.GetByCompany(companyId, role))
                .ReturnsAsync(users);

            var result = await Suite.Repository.GetByCompany(companyId, role);

            Assert.Equal(users.Count, result.Count);
        }
        
        [Fact]
        public async Task GetByCompanyResultEmployeersPropertyEqualUserModelPropery()
        {
            var commonId = 1;
            var companyId = commonId++;
            var role = "employee";

            var users = new List<UserModel>
            {
                new UserModel
                {
                    Id = commonId++,
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                    PhoneNumber = "+79999999",
                    CompanyId = companyId
                }
            };

            Suite.APIMock
                .Setup(m => m.GetByCompany(companyId, role))
                .ReturnsAsync(users);

            var result = await Suite.Repository.GetByCompany(companyId, role);

            Assert.Equal(users[0].Id, result.ElementAt(0).Id);
            Assert.Equal(users[0].FirstName, result.ElementAt(0).FirstName);
            Assert.Equal(users[0].LastName, result.ElementAt(0).LastName);
            Assert.Equal(users[0].PhoneNumber, result.ElementAt(0).PhoneNumber);
            Assert.Equal(users[0].CompanyId, result.ElementAt(0).CompanyId);
        }
    }
}