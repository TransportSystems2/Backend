using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.SignUp;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface ISignUpService : IApplicationTransactionService
    {
        Task SignUpDriverCompany(int identityUserId, DriverCompanyAM driverCompanyModel);

        Task SignUpDispatcherCompany(int identityUserId, DispatcherCompanyAM dispatcherCompanyModel);
    }
}