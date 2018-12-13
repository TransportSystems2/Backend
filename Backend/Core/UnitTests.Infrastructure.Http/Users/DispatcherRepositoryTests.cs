namespace TransportSystems.Backend.Core.UnitTests.Infrastructure.Http.Users
{
    /*
    public class DispatcherRepositoryTestSuite
    {
        public DispatcherRepositoryTestSuite()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseNpgsql(ApplicationContext.ConnectionString);

            var applicationContext =  new ApplicationContext(optionsBuilder.Options);
            DispatcherRepository = new DispatcherRepository(applicationContext);
        }

        public IDispatcherRepository DispatcherRepository { get; }
    }

    public class DispatcherRepositoryTests
    {
        
        [Fact]
        public void GetDispatchersByGarageResultDispatchers()
        {
            var suite = new DispatcherRepositoryTestSuite();

            var garage = new Garage { Id = 1 };
            var anotherGarage = new Garage { Id = 2 };

            var entities = new List<Dispatcher>
            {
                new Dispatcher { Id = 1, GarageId = garage.Id },
                new Dispatcher { Id = 2, GarageId = garage.Id },
                new Dispatcher { Id = 3, GarageId = anotherGarage.Id },
                new Dispatcher { Id = 4, GarageId = anotherGarage.Id }
            };

            var garageDispatchers = suite.DispatcherRepository.GetByCompany(garage.Id);

            Assert.Equal(2, garageDispatchers.Count());
        }
    }
    */
}