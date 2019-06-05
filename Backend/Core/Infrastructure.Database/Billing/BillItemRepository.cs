using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Billing
{
    public class BillItemRepository : Repository<BillItem>, IBillItemRepository
    {
        public BillItemRepository(ApplicationContext context)
            : base(context)
        {
        }

        public async Task<decimal> GetTotalCost(int billId)
        {
            return await Entities
                .Where(b => b.BillId.Equals(billId))
                .Select(b => b.Cost).SumAsync();
        }

        public async Task<ICollection<BillItem>> GetAll(int billId)
        {
            return await Entities
                .Where(b => b.BillId.Equals(billId))
                .ToListAsync();
        }
    }
}