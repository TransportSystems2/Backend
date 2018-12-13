using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Domain.Interfaces
{
    public interface IPushTokenRepository : IRepository<PushToken>
    {
        Task<PushToken> ByValue(string value);
    }
}