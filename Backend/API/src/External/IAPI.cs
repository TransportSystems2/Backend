using System.Threading.Tasks;
using Refit;
using TransportSystems.Backend.Application.Models.SignUp;

namespace TransportSystems.Backend.API.External
{
    public interface IAPI
    {
        [Post("/api/signup/company/")]
        Task SignUpCompany([Body(BodySerializationMethod.Json)] CompanyApplicationAM companyApplicationModel);
    }
}