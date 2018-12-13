using System;

using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces;
using System.Threading.Tasks;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Organization
{
    public class CompanyService : DomainService<Company>, ICompanyService
    {

        public CompanyService(ICompanyRepository repository, IGarageService garageService)
            : base(repository)
        {
            GarageService = garageService;
        }

        protected new ICompanyRepository Repository => (ICompanyRepository)base.Repository;

        protected IGarageService GarageService { get; }

        public async Task<Company> Create(int garageId, string name)
        {
            var companyModel = new Company { GarageId = garageId, Name = name };
            await AddCompany(companyModel);

            return companyModel;
        }

        protected async Task AddCompany(Company company)
        {
            await Verify(company);

            await Repository.Add(company);
            await Repository.Save();
        }

        protected override async Task<bool> DoVerifyEntity(Company entity)
        {
            if (!await GarageService.IsExist(entity.GarageId))
            {
                throw new EntityNotFoundException($"GarageId:{entity.GarageId} not found", "Garage");
            }

            return true;
        }
    }
}
