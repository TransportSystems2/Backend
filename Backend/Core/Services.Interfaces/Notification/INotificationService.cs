using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Core.Notification;

namespace TransportSystems.Backend.Core.Services.Interfaces.Notification
{
    public interface INotificationService : IService
    {
        Task Notice(Notify notify);
    }
}