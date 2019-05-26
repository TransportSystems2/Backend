using System.Threading.Tasks;

namespace TransportSystems.Backend.Identity.Signin.Services
{
    public interface ISlackService
    {
        Task<bool> SendAsync(string message);
    }
}
