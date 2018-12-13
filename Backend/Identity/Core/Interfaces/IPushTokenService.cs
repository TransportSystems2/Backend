using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Interfaces
{
    public interface IPushTokenService
    {
        Task<PushToken> CreateToken(string value, PushTokenType type, int userId);

        Task<ICollection<PushToken>> ReadTokens(int userId);

        Task<ICollection<PushToken>> ReadTokens(int[] usersId);

        Task<PushToken> DeleteToken(string value, PushTokenType tokenType, int userId);

        Task DeleteTokensByUser(int userId);

        Task<bool> ExistToken(string value);
    }
}