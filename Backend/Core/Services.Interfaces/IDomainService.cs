using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core;

namespace TransportSystems.Backend.Core.Services.Interfaces
{
    public interface IDomainService<T> : IService where T : BaseEntity
    {
        Task<T> Get(int id);

        Task<ICollection<T>> Get(int[] ids);

        Task<ICollection<T>> GetAll();

        Task<bool> IsExist(int id);

        Task<bool> Verify(T entity, bool isCacthException = false);
    }
}