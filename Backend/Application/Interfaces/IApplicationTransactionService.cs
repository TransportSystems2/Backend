using TransportSystems.Backend.Core.Services.Interfaces;

namespace TransportSystems.Backend.Application.Interfaces
{
    public interface IApplicationTransactionService : IApplicationBaseService
    {
        ITransactionService TransactionService { get; }
    }
}