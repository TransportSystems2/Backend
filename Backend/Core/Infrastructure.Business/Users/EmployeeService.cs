using System;
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

        public async Task<ICollection<T>> GetByCompany(int companyId)
        {
            if (!await CompanyService.IsExist(companyId))
            {
                throw new EntityNotFoundException("CompanyId");
            }

            return await Repository.GetByCompany(companyId, GetDefaultRole());
        }

        public async Task<T> AssignCompany(int id, int companyId)
        {
            var user = await Get(id);
            if (user == null)
            {
                throw new ArgumentException($"Id:{id} is null", "Id");
            }

            if (!await CompanyService.IsExist(companyId))
            {
                throw new EntityNotFoundException($"Company with id = {companyId}, doesn't exist", "Company");
            }

            user.CompanyId = companyId;

            await Repository.Update(user);
            await Repository.Save();

            return user;
        }
    }
}