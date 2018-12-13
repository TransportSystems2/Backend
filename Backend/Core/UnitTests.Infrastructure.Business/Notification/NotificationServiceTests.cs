namespace TransportSystems.UnitTests.Infrastructure.Business.Notification
{
    public class NotificationServiceTests
    {
        /*
        [Fact]
        public void ProcessCreatedOrderResultNoticedModerator()
        {
            var orderServiceMock = new Mock<IOrderService>();
            var notificationService = new NotificationService(orderServiceMock.Object);

            var wasIsHit = false;
            notificationService.UserWasNoticed += delegate { wasIsHit = true; };
            var order = new Order
            {
                Garage = new Garage
                {
                    Moderator = new Moderator()
                }
            };

            orderServiceMock.Raise(mock => mock.AddedOrder += null, orderServiceMock.Object, order);

            Assert.True(wasIsHit);
        }
        */
    }
}