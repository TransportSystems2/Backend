using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Services.Interfaces.RegistrationNumber;

namespace TransportSystems.Backend.Core.Infrastructure.Business.RegistrationNumber
{
    public class RegistrationNumberService : IRegistrationNumberService
    {
        public Task<bool> ValidRegistrationNumber(string registrationNumber)
        {
            return Task.FromResult(Regex.Match(registrationNumber, @"^[АВЕКМНОРСТУХ]\d{3}(?<!000)[АВЕКМНОРСТУХ]{2}\d{2,3}$").Success);
        }
    }
}