using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.SignUp;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface ISignUpService : IApplicationTransactionService
    {
        Task SignUpDriverCompany(DriverCompanyAM driverCompanyModel);

        Task SignUpDispatcherCompany(DispatcherCompanyAM dispatcherCompanyModel);
    }
}