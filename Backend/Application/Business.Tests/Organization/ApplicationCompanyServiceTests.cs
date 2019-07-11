using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TransportSystems.Backend.Application.Business.Organization;
using TransportSystems.Backend.Application.Business.Tests.Suite;
using TransportSystems.Backend.Application.Interfaces;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;
using Xunit;

namespace TransportSystems.Backend.Application.Business.Tests.Organization
{
    public class ApplicationCompanyServiceTestSuite : TransactionTestsSuite
    {
        public ApplicationCompanyServiceTestSuite()
        {
            DomainCompanyServiceMock = new Mock<ICompanyService>();
            DomainDriverServiceMock = new Mock<IDriverService>();
            VehicleServiceMock = new Mock<IApplicationVehicleService>();

            Service = new ApplicationCompanyService(
                TransactionServiceMock.Object,
                MappingService,
                DomainCompanyServiceMock.Object,
                DomainDriverServiceMock.Object,
                VehicleServiceMock.Object);
        }

        public IApplicationCompanyService Service { get; }

        public Mock<ICompanyService> DomainCompanyServiceMock { get; }

        public Mock<IDriverService> DomainDriverServiceMock { get; }

        public Mock<IApplicationVehicleService> VehicleServiceMock { get; }
    }

    public class ApplicationCompanyServiceTests : BaseServiceTests<ApplicationCompanyServiceTestSuite>
    {
        [Fact]
        public async Task CreateDomainCompany()
        {
            var domainCompany = new Company
            {
                Name = "ГосЭвакуатор"
            };

            Suite.DomainCompanyServiceMock
                .Setup(m => m.Create(domainCompany.Name))
                .ReturnsAsync(domainCompany);

            var result = await Suite.Service.CreateDomainCompany(domainCompany.Name);

            Assert.Equal(domainCompany.Name, result.Name);
        }

        [Fact]
        public async Task GetDomainCompany()
        {
            var domainCompany = new Company
            {
                Name = "ГосЭвакуатор"
            };

            Suite.DomainCompanyServiceMock
                .Setup(m => m.GetByName(domainCompany.Name))
                .ReturnsAsync(domainCompany);

            var result = await Suite.Service.GetDomainCompany(domainCompany.Name);

            Assert.Equal(domainCompany, result);
        }

        [Fact]
        public async Task GetDrivers_Result_DriversCountEqualsDomainDriversCount()
        {
            var commonId = 1;

            var companyId = commonId++;
            var domainDrivers = new List<Driver> {
                new Driver (),
                new Driver ()
            };

            Suite.DomainDriverServiceMock
                .Setup(m => m.GetByCompany(companyId))
                .ReturnsAsync(domainDrivers);

            var result = await Suite.Service.GetDrivers(companyId);

            Assert.Equal(domainDrivers.Count, result.Count);
        }

        [Fact]
        public async Task GetDrivers_Result_DriverCorrespondsDomainDriver()
        {
            var commonId = 1;

            var companyId = commonId++;
            var domainDrivers = new List<Driver> {
                new Driver {
                Id = commonId++,
                FirstName = "FristName1",
                LastName = "LastName1",
                AddedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
                }
            };

            Suite.DomainDriverServiceMock
                .Setup(m => m.GetByCompany(companyId))
                .ReturnsAsync(domainDrivers);

            var result = await Suite.Service.GetDrivers(companyId);

            Assert.Equal(domainDrivers.Count, result.Count);

            Assert.Equal(domainDrivers[0].Id, result.ElementAt(0).Id);
            Assert.Equal(domainDrivers[0].FirstName, result.ElementAt(0).FirstName);
            Assert.Equal(domainDrivers[0].LastName, result.ElementAt(0).LastName);
            Assert.Equal(domainDrivers[0].AddedDate, result.ElementAt(0).AddedDate);
            Assert.Equal(domainDrivers[0].ModifiedDate, result.ElementAt(0).ModifiedDate);
        }

        [Fact]
        public async Task GetVehicles()
        {
            var companyId = 1;

            var vehicles = new List<VehicleAM>();

            Suite.VehicleServiceMock
                .Setup(m => m.GetByCompany(companyId))
                .ReturnsAsync(vehicles);

            var result = await Suite.Service.GetVehicles(companyId);

            Assert.Equal(vehicles, result);
        }
    }
}