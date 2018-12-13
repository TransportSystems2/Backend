using Moq;
using System.Threading.Tasks;
using TransportSystems.Backend.Core.Domain.Interfaces;
using TransportSystems.Backend.Core.Services.Interfaces;

namespace TransportSystems.Backend.Application.UnitTests.Business.Suite
{
    public abstract class TransactionTestsSuite : MappingTestsSuite
    {
        public TransactionTestsSuite()
        {
            TransactionServiceMock = new Mock<ITransactionService>();
            TransactionMock = new Mock<ITransaction>();

            TransactionServiceMock
                 .Setup(m => m.BeginTransaction())
                 .ReturnsAsync(TransactionMock.Object);
        }

        public Mock<ITransactionService> TransactionServiceMock { get; }

        public Mock<ITransaction> TransactionMock { get; }
    }
}