using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Core.Domain.Core;
using TransportSystems.Backend.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Core.Infrastructure.Database
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        public Repository(ApplicationContext context)
        {
            Context = context;
            Entities = Context.Set<T>();
        }

        protected DbSet<T> Entities { get; set; }

        protected ApplicationContext Context { get; }

        public virtual async Task<ICollection<T>> GetAll()
        {
            return await Entities.ToListAsync();
        }

        public virtual Task<T> Get(int id)
        {
            return Entities.SingleOrDefaultAsync(e => e.Id.Equals(id));
        }

        public async Task<ICollection<T>> Get(int[] idArray)
        {
            return await Entities.Where(e => idArray.Contains(e.Id)).ToListAsync();
        }

        public virtual Task Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return Entities.AddAsync(entity);
        }

        public virtual Task AddRange(ICollection<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return Entities.AddRangeAsync(entities);
        }

        public virtual Task AddRange(params T[] entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return Entities.AddRangeAsync(entities);
        }

        public virtual Task Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Entities.Update(entity);

            return Task.CompletedTask;
        }

        public virtual Task Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Entities.Remove(entity);

            return Task.CompletedTask;
        }

        public virtual Task Save()
        {
            return Context.SaveChangesAsync();
        }

        public Task<bool> IsExist(int id)
        {
            return Entities.AnyAsync(e => e.Id.Equals(id));
        }
    }
}