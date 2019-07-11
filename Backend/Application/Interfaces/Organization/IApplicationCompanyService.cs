using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.Transport;
using TransportSystems.Backend.Application.Models.Users;
using TransportSystems.Backend.Core.Domain.Core.Organization;

namespace TransportSystems.Backend.Application.Interfaces.Organization
{
    public interface IApplicationCompanyService : IApplicationTransactionService
    {
        Task<Company> CreateDomainCompany(string name);

        Task<Company> GetDomainCompany(string name);

        Task<ICollection<DriverAM>> GetDrivers(int companyId);

        Task<ICollection<VehicleAM>> GetVehicles(int companyId);
    }
}