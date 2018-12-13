using System.Threading.Tasks;

namespace TransportSystems.Backend.Core.Services.Interfaces.Trading
{
    public interface IMarketService : IService
    {
        Task LaunchTrading();

        Task Trade(int orderId);
    }
}