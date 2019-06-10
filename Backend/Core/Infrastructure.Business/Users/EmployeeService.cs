using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces.Organization;
using TransportSystems.Backend.Core.Services.Interfaces.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Users
{
    public abstract class EmployeeService<T> :
        IdentityUserService<T>,
        IEmployeeService<T>
        where T : Employee, new ()
    {
        public EmployeeService(
            IEmployeeRepository<T> repository,
            ICompanyService companyService)
            : base(repository)
        {
            CompanyService = companyService;
        }

        protected new IEmployeeRepository<T> Repository => (IEmployeeRepository<T>)base.Repository;

        protected ICompanyService CompanyService { get; }

        public async Task<ICollection<T>> GetByCompany(int companyId, string role)
        {
            if (!await CompanyService.IsExist(companyId))
            {
                throw new EntityNotFoundException("CompanyId");
            }

            return await Repository.GetByCompany(companyId, role);
        }

        public async Task<T> Create(string firstName, string lastName, string phoneNumber, int companyId)
        {
            if (!await CompanyService.IsExist(companyId))
            {
                throw new EntityNotFoundException($"Company with id = {companyId}, doesn't exist", "Company");
            }

            var result = await Create(firstName, lastName, phoneNumber);
            result.CompanyId = companyId;

            await Repository.Update(result);
            await Repository.Save();

            return result;
        }
    }
}