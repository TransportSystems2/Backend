using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationCompanyService : IApplicationTransactionService
    {
        Task<Company> CreateDomainCompany(string name);

        Task<Company> GetDomainCompany(string name);
    }
}