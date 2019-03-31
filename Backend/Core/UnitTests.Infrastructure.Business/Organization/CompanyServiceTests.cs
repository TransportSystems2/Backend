﻿using Moq;
using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Infrastructure.Business.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using Xunit;

namespace TransportSystems.UnitTests.Infrastructure.Business.Oraganization
{
    public class CompanyServiceTestSuite
    {
        public CompanyServiceTestSuite()
        {
            RepositoryMock = new Mock<ICompanyRepository>();

            Service = new CompanyService(RepositoryMock.Object);
        }

        public Mock<ICompanyRepository> RepositoryMock { get; }

        public ICompanyService Service { get; }
    }

    public class CompanyServiceTest
    {
        public CompanyServiceTest()
        {
            Suite = new CompanyServiceTestSuite();
        }

        public CompanyServiceTestSuite Suite { get; }

        [Fact]
        public async Task CreateCompany()
        {
            var companyName = "Транспортные Системы";
            var isPrivate = false;

            Suite.RepositoryMock
                .Setup(m => m.GetByName(companyName))
                .Returns(Task.FromResult<Company>(null));

            var resultCompany = await Suite.Service.Create(companyName, isPrivate);

            Assert.NotNull(resultCompany);
            Assert.Equal(companyName, resultCompany.Name);
            Assert.Equal(isPrivate, resultCompany.IsPrivate);
        }

        [Fact]
        public async Task CreateCompanyWhenCompanyWithSameNameAlreadyExists()
        {
            var existingCompany = new Company
            {
                Id = 1,
                Name = "Транспортные Системы"
            };

            var isPrivate = false;

            Suite.RepositoryMock
                .Setup(m => m.GetByName(existingCompany.Name))
                .ReturnsAsync(existingCompany);

            await Assert.ThrowsAsync<EntityAlreadyExistsException>("Name", () => Suite.Service.Create(existingCompany.Name, isPrivate));
        }

        [Fact]
        public async Task CreatCompanyWhenNameIsEmptyOrNull()
        {
            var name = "";
            var isPrivate = false;

            await Assert.ThrowsAsync<ArgumentNullException>("Name", () => Suite.Service.Create(name, isPrivate));
        }

        [Fact]
        public async Task GetCompanyByName()
        {
            var company = new Company { Name = "Sample" };

            Suite.RepositoryMock
                .Setup(m => m.GetByName(company.Name))
                .ReturnsAsync(company);

            var result = await Suite.Service.GetByName(company.Name);

            Assert.Equal(company, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetCompanyByNameWhereNameIsEmptyOrNull(string name)
        {
            await Assert.ThrowsAsync<ArgumentException>("Name", () => Suite.Service.GetByName(name));
        }
    }
}