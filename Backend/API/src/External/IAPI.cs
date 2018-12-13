using System.Threading.Tasks;
using Refit;
using TransportSystems.Backend.Application.Models.SignUp;

namespace TransportSystems.Backend.API.External
{
    public interface IAPI
    {
        [Post("/api/signup/dispatcher/")]
        Task SignUpDispatcher([Body(BodySerializationMethod.Json)] DispatcherCompanyAM dispatcherCompanyModel);

        [Post("/api/signup/driver/")]
        Task SignUpDriver([Body(BodySerializationMethod.Json)] DriverCompanyAM driverCompanyModel);
    }
}