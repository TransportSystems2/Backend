using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Mapping;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Application.Business.Organization
{
    public class ApplicationCompanyService :
        ApplicationTransactionService,
        IApplicationCompanyService
    {
        public ApplicationCompanyService(
            ITransactionService transactionService,
            IMappingService mappingService,
            ICompanyService domainCompanyService,
            IDriverService domainDriverService)
        : base(transactionService)
        {
            MappingService = mappingService;
            DomainCompanyService = domainCompanyService;
            DomainDriverService = domainDriverService;
        }

        protected IMappingService MappingService { get; }

        protected ICompanyService DomainCompanyService { get; }
        
        protected IDriverService DomainDriverService { get; }

        public Task<Company> CreateDomainCompany(string name)
        {
            return DomainCompanyService.Create(name);
        }

        public Task<Company> GetDomainCompany(string name)
        {
            return DomainCompanyService.GetByName(name);
        }

        public async Task<ICollection<DriverAM>> GetDrivers(int companyId)
        {
            var domainDrivers = await DomainDriverService.GetByCompany(companyId);
            var drivers = new List<DriverAM>();

            return MappingService.Map<IEnumerable<Driver>, ICollection<DriverAM>>(domainDrivers, drivers);
        }
    }
}