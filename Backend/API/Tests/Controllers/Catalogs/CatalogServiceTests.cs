using Moq;
using TransportSystems.Backend.API.Controllers.Catalogs;
using TransportSystems.Backend.Application.Interfaces.Catalogs;

namespace TransportSystems.Backend.API.Tests.Controllers.Catalogs
{
    public class CatalogControllerTestSuite
    {
        public CatalogControllerTestSuite()
        {
            CatalogService = new Mock<IApplicationCatalogService>();
            CatalogController = new CatalogController(CatalogService.Object);
        }

        public Mock<IApplicationCatalogService> CatalogService {get;}

        public CatalogController CatalogController { get; }
    }

    public class CatalogServiceTests
    {
    }
}
