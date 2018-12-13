using System;

namespace TransportSystems.Backend.Core.Domain.Interfaces
{
    public interface ITransaction : IDisposable
    {
        Guid TransactionId
        {
            get;
        }

        void Commit();

        void Rollback();
    }
}