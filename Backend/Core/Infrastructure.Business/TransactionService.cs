using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Business
{
    public class TransactionService : BaseService, ITransactionService
    {
        public TransactionService(IContext context)
        {
            Context = context;
        }

        protected IContext Context { get; }

        public Task<ITransaction> BeginTransaction()
        {
            return Context.BeginTransaction();
        }
    }
}