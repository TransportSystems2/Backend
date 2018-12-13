using System;
using Microsoft.EntityFrameworkCore.Storage;
using TransportSystems.Backend.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Database
{
    public class Transaction : ITransaction
    {
        public Transaction(IDbContextTransaction contextTransaction)
        {
            ContextTransaction = contextTransaction;
        }

        protected IDbContextTransaction ContextTransaction { get; }

        public Guid TransactionId => ContextTransaction.TransactionId;

        public void Commit()
        {
            ContextTransaction.Commit();
        }

        public void Rollback()
        {
            ContextTransaction.Rollback();
        }

        public void Dispose()
        {
            ContextTransaction.Dispose();
        }
    }
}