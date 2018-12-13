using TransportSystems.Backend.Application.UnitTests.Business.Suite;

namespace TransportSystems.Backend.Application.UnitTests.Business
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