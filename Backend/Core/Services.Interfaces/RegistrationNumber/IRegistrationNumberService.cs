using System.Threading.Tasks;

namespace TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber
{
    public interface IRegistrationNumberService
    {
        Task<bool> ValidRegistrationNumber(string registrationNumber);
    }
}
