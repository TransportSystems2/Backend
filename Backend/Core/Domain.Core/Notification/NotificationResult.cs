using TransportSystems.Backend.Core.Domain.Core.Enums;

namespace TransportSystems.Backend.Core.Domain.Core.Notification
{
    public struct NotificationResult
    {
        public NotificationResult(MessageStatus status)
        {
            Status = status;
        }

        public MessageStatus Status { get; }
    }
}