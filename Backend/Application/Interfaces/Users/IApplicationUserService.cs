using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Interfaces.Users
{
    public interface IApplicationUserService : IApplicationTransactionService
    {
        Task<Customer> GetOrCreateDomainCustomer(CustomerAM customer);

        Task<Moderator> GetDomainModeratorByIdentityUser(int identityUserId);

        Task<Dispatcher> GetDomainDispatcherByIdentityUser(int identityUserId);

        Task<Dispatcher> GetDomainDispatcher(int dispatcherId);
    }
}