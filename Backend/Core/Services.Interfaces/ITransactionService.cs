using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Core.Services.Interfaces
{
    public interface ITransactionService : IService
    {
        Task<ITransaction> BeginTransaction();
    }
}