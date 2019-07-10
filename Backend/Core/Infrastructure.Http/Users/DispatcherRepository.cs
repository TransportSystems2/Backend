using AutoMapper;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Core.Domain.Interfaces.Users;
using TransportSystems.Backend.Identity.API;

namespace TransportSystems.Backend.Core.Infrastructure.Http.Users
{
    public class DispatcherRepository : EmployeeRepository<Dispatcher>, IDispatcherRepository
    {
        public DispatcherRepository(IIdentityUsersAPI identityUsersAPI)
            : base(identityUsersAPI)
        {
        }
    }
}