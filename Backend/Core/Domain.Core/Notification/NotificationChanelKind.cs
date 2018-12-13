using System;

namespace TransportSystems.Backend.Core.Domain.Core.Notification
{
    [Flags]
    public enum NotificationChanelKind
    {
        /// <summary>
        /// Email
        /// </summary>
        Email = 1,

        /// <summary>
        /// Sms
        /// </summary>
        Sms = 2,

        /// <summary>
        /// Viber
        /// </summary>
        Viber = 4,

        /// <summary>
        /// All
        /// </summary>
        All = Email | Sms | Viber
    }
}