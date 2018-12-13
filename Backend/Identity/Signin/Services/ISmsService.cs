using System.Threading.Tasks;

namespace TransportSystems.Backend.Identity.Signin.Services
{
    public interface ISmsService
    {
        Task<bool> SendAsync(string phoneNumber, string body);
    }
}
