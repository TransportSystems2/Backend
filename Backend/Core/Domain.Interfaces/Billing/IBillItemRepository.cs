using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;

namespace TransportSystems.Backend.Core.Domain.Interfaces.Billing
{
    public interface IBillItemRepository : IRepository<BillItem>
    {
        Task<decimal> GetTotalCost(int billId);

        Task<ICollection<BillItem>> GetAll(int billId);
    }
}