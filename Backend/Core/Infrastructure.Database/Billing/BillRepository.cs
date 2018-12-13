using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Core.Domain.Interfaces.Billing;

namespace TransportSystems.Backend.Core.Infrastructure.Database.Billing
{
    public class BillRepository : Repository<Bill>, IBillRepository
    {
        public BillRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}