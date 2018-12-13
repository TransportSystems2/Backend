using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

using TransportSystems.Backend.Core.Services.Interfaces.Notification.Chanels;
using TransportSystems.Backend.Core.Domain.Core.Notification;
using TransportSystems.Backend.Core.Domain.Core.Enums;

namespace TransportSystems.Backend.Core.Infrastructure.Business.Notification.Chanels
{
    public class EmailNotificationChanel : IEmailNotificationChanel
    {
        public async Task<NotificationResult> SendAsync(string from, IEnumerable<string> to, string subject, string body)
        {
            var client = new SmtpClient("mail.gosevakuator.ru")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mail@gosevakuator.ru", "982324Pa")
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                Body = body,
                Subject = subject
            };

            mailMessage.To.Add(string.Join(";", to));

            var status = MessageStatus.Non;

            try
            {
                await client.SendMailAsync(mailMessage);
                status = MessageStatus.Sent;
            }
            catch
            {
                status = MessageStatus.Error;
            }

            var result = new NotificationResult(status);

            return result;
        }
    }
}