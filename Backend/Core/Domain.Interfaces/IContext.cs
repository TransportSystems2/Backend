using System.Threading.Tasks;

namespace TransportSystems.Backend.Core.Domain.Interfaces
{
    public interface IContext
    {
        Task<ITransaction> BeginTransaction();
    }
}