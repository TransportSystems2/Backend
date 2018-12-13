using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;
using TransportSystems.Backend.Identity.Core.Domain.Interfaces;

namespace TransportSystems.Backend.Identity.Core.Database
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        public Repository(IdentityContext context)
        {
            Context = context;
            Entities = Context.Set<T>();
        }

        protected DbSet<T> Entities { get; set; }

        protected IdentityContext Context { get; }

        public async virtual Task Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Entities.AddAsync(entity);
        }

        public virtual Task<T> Get(int id)
        {
            return Entities.SingleOrDefaultAsync(e => e.Id.Equals(id));
        }

        public async virtual Task<ICollection<T>> Get(int[] ids)
        {
            return await Entities.Where(e => ids.Contains(e.Id)).ToListAsync();
        }

        public virtual Task Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Entities.Update(entity);

            return Task.FromResult(entity);
        }

        public virtual Task Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Entities.Remove(entity);

            return Task.FromResult(entity);
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