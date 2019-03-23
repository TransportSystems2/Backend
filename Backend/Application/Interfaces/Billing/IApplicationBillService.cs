using Common.Models.Units;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Billing;
using TransportSystems.Backend.Application.Models.Billing;

namespace TransportSystems.Backend.Application.Interfaces.Billing
{
    public interface IApplicationBillService : IApplicationTransactionService
    {
        Task<decimal> GetTotalCost(int billId);

        Task<Bill> CreateDomainBill(BillAM bill);

        Task<BillItem> CreateDomainBillItem(int billId, BillItemAM billItem);

        Task<BillAM> CalculateBill(BillInfoAM billInfo, BasketAM basket);

        Task<BillItemAM> CalculateBillItem(string key, int value, decimal price, float degreeOfDifficulty);

        Task<BillInfoAM> GetBillInfo(Coordinate coordinate, int catalogItemId);
    }
}