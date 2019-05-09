//using System.Collections.Generic;
//using TransportSystems.Infrastructure.Business.Notification.Chanels;
//using Xunit;

//namespace TransportSystems.Infrastructure.Business.Tests.Notification
//{
//    public class EmailNotificationChanelTests
//    {
//        [Fact]
//        public async void SendEmailResultEmailIsSent()
//        {
//            var emailChanel = new EmailNotificationChanel();
//            var result = await emailChanel.SendAsync("mail@gosevakuator.ru", new List<string> { "fedorkinp@yandex.ru" }, "body", "subject");

//            Assert.True(result.Status == Domain.Core.Enums.MessageStatus.Sent);
//        }
//    }
//}