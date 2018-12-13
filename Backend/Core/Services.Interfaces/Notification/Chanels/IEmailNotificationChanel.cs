using System.Collections.Generic;
using System.Threading.Tasks;

using TransportSystems.Backend.Core.Domain.Core.Notification;

namespace TransportSystems.Backend.Core.Services.Interfaces.Notification.Chanels
{
    public interface IEmailNotificationChanel
    {
        Task<NotificationResult> SendAsync(string from, IEnumerable<string> to, string subject, string body);
    }
}
