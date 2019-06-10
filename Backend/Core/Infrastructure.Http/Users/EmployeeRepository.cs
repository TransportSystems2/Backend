using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Identity.API;
using TransportSystems.Backend.Identity.Core.Data.External.Users;

namespace TransportSystems.Backend.Core.Infrastructure.Http.Users
{
    public class EmployeeRepository<T> : IdentityUserRepository<T>, IEmployeeRepository<T> where T : Employee
    {
        public EmployeeRepository(
            IIdentityUsersAPI identityUsersAPI,
            IMapper mapperService)
            : base(identityUsersAPI)
        {
            MapperService = mapperService;
        }

        protected IMapper MapperService { get; }

        public async Task<ICollection<T>> GetByCompany(int companyId, string role)
        {
            var users = await IdentityUsersAPI.GetByCompany(companyId, role);

            //return MapperService.Map<ICollection<T>>(users);
            var result = new List<T>();
            foreach (var user in users)
            {
                var entity = MapperService.Map<T>(user);
                result.Add(entity);
            }

            return result;
        }
    }
}