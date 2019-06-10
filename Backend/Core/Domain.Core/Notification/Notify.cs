using System.Collections.Generic;

using TransportSystems.Backend.Core.Domain.Core.Users;

namespace TransportSystems.Backend.Core.Domain.Core.Notification
{
    public struct Notify
    {
        public Notify(IEnumerable<IdentityUser> users, string subject, string text, NotificationChanelKind chanelKind)
        {
            Users = users;
            Subject = subject;
            Text = text;
            ChanelKind = chanelKind;
        }

        public IEnumerable<IdentityUser> Users { get; }

        public string Subject { get; }

        public string Text { get; }

        public NotificationChanelKind ChanelKind { get; }
    }
}