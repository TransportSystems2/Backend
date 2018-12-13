using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Database;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain.Exceptions;
using TransportSystems.Backend.Identity.Core.Domain.Interfaces;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Core.Business
{
    public class Service<TEntity, TRepository> : IService<TEntity> where TRepository : IRepository<TEntity> where TEntity : BaseEntity
    {
        public Service(TRepository repository)
        {
            Repository = repository;
        }

        public TRepository Repository { get; }

        public async Task<TEntity> Create(TEntity entity)
        {
            if (!await ValidateEntity(entity))
            {
                throw new ValidateException($"Entity isn't valid: {entity.GetType().Name}");
            }

            await Repository.Add(entity);
            await Repository.Save();

            return entity;
        }

        public async Task<TEntity> Delete(TEntity entity)
        {
            await Repository.Remove(entity);
            await Repository.Save();

            return entity;
        }

        public Task<TEntity> Read(int id)
        {
            return Repository.Get(id);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            await Repository.Update(entity);

            return entity;
        }

        protected virtual Task<bool> ValidateEntity(TEntity entity)
        {
            return Task.FromResult(false);
        }
    }
}