using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Core.Domain.Core.Organization;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationCompanyService : IApplicationTransactionService
    {
        Task<Company> CreateDomainCompany(string name);

        Task<Company> GetDomainCompany(string name);

        Task<Dispatcher> CreateDispatcher(int companyDispatcherId, DispatcherAM newDispatcher);
    }
}