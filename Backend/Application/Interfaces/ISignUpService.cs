using System.Threading.Tasks;
using TransportSystems.Backend.Application.Models.SignUp;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface ISignUpService : IApplicationTransactionService
    {
        Task SignUpCompany(CompanyApplicationAM companyApplication);
    }
}