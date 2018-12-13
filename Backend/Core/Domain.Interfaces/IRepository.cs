using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core;

namespace TransportSystems.Backend.Core.Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<ICollection<T>> GetAll();

        Task<T> Get(int id);

        Task<ICollection<T>> Get(int[] idArray);

        Task Add(T entity);

        Task AddRange(params T[] entities);

        Task AddRange(ICollection<T> entities);

        Task Update(T entity);

        Task Remove(T entity);

        Task<bool> IsExist(int id);

        Task Save();
    }
}
