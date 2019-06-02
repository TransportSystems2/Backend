using TransportSystems.Backend.Application.Business.Tests.Suite;

namespace TransportSystems.Backend.Application.Business.Tests
{
    public class BaseServiceTests <TSuite> where TSuite : BaseTestsSuite, new()
    {
        public BaseServiceTests()
        {
            Suite = new TSuite();
        }

        public TSuite Suite { get; }
    }
}