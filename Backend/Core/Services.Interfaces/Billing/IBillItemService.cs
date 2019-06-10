using System.Collections.Generic;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;

namespace TransportSystems.Backend.Core.Services.Interfaces.Billing
{
    public interface IBillItemService : IDomainService<BillItem>
    {
        Task<BillItem> Create(
            int billId,
            string key,
            int value,
            decimal price,
            decimal cost);

        Task UpdateTotalCost(int billId);

        Task<ICollection<BillItem>> GetAll(int billId);
    }
}