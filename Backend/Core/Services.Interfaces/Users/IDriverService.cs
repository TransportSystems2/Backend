using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Services.Interfaces.Users
{
    public interface IDriverService : IEmployeeService<Driver>
    {
        Task<Driver> AssignVehicle(int driverId, int vehicleId);
    }
}