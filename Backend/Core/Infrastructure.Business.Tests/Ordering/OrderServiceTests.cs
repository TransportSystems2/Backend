using Moq;
using TransportSystems.Backend.Core.Domain.Interfaces.Ordering;
using TransportSystems.Backend.Core.Infrastructure.Business;
using TransportSystems.Backend.Core.Services.Interfaces.Interfaces;

using Xunit;

namespace TransportSystems.Infrastructure.Business.Tests
{
    public class OrderServiceTestSuite
    {
        public OrderServiceTestSuite()
        {
            RepositoryMock = new Mock<IOrderRepository>();
            Service = new OrderService(RepositoryMock.Object);
        }

        public IOrderService Service { get; }

        public Mock<IOrderRepository> RepositoryMock { get; }
    }

    public class OrderServiceTest
    {
        [Fact]
        public async void CreateOrder()
        {
            var suite = new OrderServiceTestSuite();

            var newOrder = await suite.Service.Create();

            suite.RepositoryMock
                .Verify(m => m.Add(newOrder));
        }
    }
}