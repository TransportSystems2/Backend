using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;
using TransportSystems.Backend.Application.Models.Users;

namespace TransportSystems.Backend.Application.Interfaces.Users
{
    public interface IApplicationCustomerService : IApplicationTransactionService
    {
        Task<Customer> GetDomainCustomer(CustomerAM customer);
    }
}