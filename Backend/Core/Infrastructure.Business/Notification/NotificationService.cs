using TransportSystems.Backend.Core.Services.Interfaces.Notification;
using TransportSystems.Backend.Core.Domain.Core.Notification;
using System.Threading.Tasks;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Notification
{
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService()
        {
        }

        public Task Notice(Notify notify)
        {
            return Task.CompletedTask;
        }
    }
}