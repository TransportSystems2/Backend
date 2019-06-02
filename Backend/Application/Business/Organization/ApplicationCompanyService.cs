using System;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Interfaces.Organization;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;

namespace TransportSystems.Backend.Application.Business.Organization
{
    public class ApplicationCompanyService :
        ApplicationTransactionService,
        IApplicationCompanyService
    {
        public ApplicationCompanyService(
            ITransactionService transactionService,
            ICompanyService domainCompanyService)
        : base(transactionService)
        {
            DomainCompanyService = domainCompanyService;
        }

        protected ICompanyService DomainCompanyService { get; }

        public Task<Company> CreateDomainCompany(string name)
        {
            return DomainCompanyService.Create(name);
        }

        public Task<Dispatcher> CreateDispatcher(int companyDispatcherId, DispatcherAM newDispatcher)
        {
            throw new NotImplementedException();
        }

        public Task<Company> GetDomainCompany(string name)
        {
            return DomainCompanyService.GetByName(name);
        }
    }
}