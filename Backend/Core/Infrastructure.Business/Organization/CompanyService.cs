using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using System.Threading.Tasks;
using System;
using TransportSystems.Backend.Core.Services.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Organization
{
    public class CompanyService : DomainService<Company>, ICompanyService
    {
        public CompanyService(ICompanyRepository repository)
            : base(repository)
        {
        }

        protected new ICompanyRepository Repository => (ICompanyRepository)base.Repository;

        public Task AssignModerator(int companyId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Company> Create(string name, bool isPrivate = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name of company is null or empty", "Name");
            }

            if (await Repository.GetByName(name) != null)
            {
                throw new EntityAlreadyExistsException($"Company this same name: {name}, alredy exists.", "Name");
            }

            var companyModel = new Company { Name = name, IsPrivate = isPrivate };
            await AddCompany(companyModel);

            return companyModel;
        }

        public async Task<Company> GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"Name is empty or null", "Name");
            }

            return await Repository.GetByName(name);
        }

        protected async Task AddCompany(Company company)
        {
            await Verify(company);

            await Repository.Add(company);
            await Repository.Save();
        }

        protected override async Task<bool> DoVerifyEntity(Company entity)
        {
            return true;
        }
    }
}
