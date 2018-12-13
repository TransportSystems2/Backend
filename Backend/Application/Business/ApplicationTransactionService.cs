using TransportSystems.Backend.Core.Services.Interfaces;
using TransportSystems.Backend.Application.Interfaces;

namespace TransportSystems.Backend.Application.Business
{
    public abstract class ApplicationTransactionService : ApplicationBaseService, IApplicationTransactionService
    {
        public ApplicationTransactionService(ITransactionService transactionService)
        {
            TransactionService = transactionService;
        }

        public ITransactionService TransactionService { get; }
    }
}