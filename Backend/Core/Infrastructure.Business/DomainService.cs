using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Business
{
    public abstract class DomainService<T> : BaseService, IDomainService<T> where T : BaseEntity
    {
        public DomainService(IRepository<T> repository)
        {
            Repository = repository;
        }

        protected IRepository<T> Repository { get; set; }

        protected abstract Task<bool> DoVerifyEntity(T entity);

        public Task<bool> Verify(T entity, bool isCacthException = false)
        {
            Task<bool> result;
            try
            {
                result = DoVerifyEntity(entity);
            }
            catch
            {
                result = Task.FromResult(false);

                if (!isCacthException)
                {
                    throw;
                }
            }

            return result;
        }

        public Task<T> Get(int id)
        {
            return Repository.Get(id);
        }

        public Task<ICollection<T>> Get(int[] ids)
        {
            return Repository.Get(ids);
        }

        public Task<ICollection<T>> GetAll()
        {
            return Repository.GetAll();
        }

        public Task<bool> IsExist(int id)
        {
            return Repository.IsExist(id);
        }
    }
}