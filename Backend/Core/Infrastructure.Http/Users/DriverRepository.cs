using AutoMapper;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Identity.API;

namespace TransportSystems.Backend.Core.Infrastructure.Http.Users
{
    public class DriverRepository : EmployeeRepository<Driver>, IDriverRepository
    {
        public DriverRepository(
            IIdentityUsersAPI identityUsersAPI,
            IMapper mapperService)
            : base(identityUsersAPI, mapperService)
        {
        }
    }
}