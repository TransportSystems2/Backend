using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Domain.Data.Domain;

namespace TransportSystems.Backend.Identity.Core.Interfaces
{
    public interface IService<T> where T : BaseEntity
    {
        Task<T> Create(T entity);

        Task<T> Read(int id);

        Task<T> Update(T entity);

        Task<T> Delete(T entity);
    }
}
