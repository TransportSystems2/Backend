using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task Add(T entity);

        Task<T> Get(int id);

        Task<ICollection<T>> Get(int[] ids);

        Task Update(T entity);

        Task Remove(T entity);

        Task<bool> IsExist(int id);

        Task Save();
    }
}
