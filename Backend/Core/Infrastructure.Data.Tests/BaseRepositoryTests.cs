using Microsoft.EntityFrameworkCore;
using System;
using TransportSystems.Backend.Core.Domain.Core;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Infrastructure.Database;

namespace TransportSystems.Backend.Core.Infrastructure.Data.Tests
{
    public abstract class BaseRepositoryTests<TRepository, TEntity> : IDisposable
        where TRepository : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        public BaseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new ApplicationContext(options);
            Context.Database.EnsureCreated();

            Repository = CreateRepository(Context);
        }

        protected ApplicationContext Context { get; }

        protected TRepository Repository { get; }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }

        protected abstract TRepository CreateRepository(ApplicationContext context);
    }
}