using AutoMapper;
using Moq;
using TransportSystems.Backend.Application.Business.Mapping;
using TransportSystems.Backend.Application.Interfaces.Mapping;
using Xunit;

namespace TransportSystems.Backend.Application.UnitTests.Business
{
    public class ApplicationMappingServiceTestsSuite
    {
        public ApplicationMappingServiceTestsSuite()
        {
            Mapper = new Mock<IMapper>();
            MappingService = new ApplicationMappingService(Mapper.Object);
        }

        public Mock<IMapper> Mapper { get; }

        public IMappingService MappingService { get; }

        public class TestDTO
        {
            public string Name { get; set; }

            public int Value { get; set; }
        }

        public class Test
        {
            public string Name { get; set; }

            public int Value { get; set; }
        }
    }

    public class ApplicationMappingServiceTests
    {
        public ApplicationMappingServiceTests()
        {
            Suite = new ApplicationMappingServiceTestsSuite();
        }

        protected ApplicationMappingServiceTestsSuite Suite { get; }

        [Fact]
        public void MapWithGeneric()
        {
            var name = "Name";
            var value = 1;

            var dto = new ApplicationMappingServiceTestsSuite.TestDTO
            {
                Name = name,
                Value = value
            };

            var dest = new ApplicationMappingServiceTestsSuite.Test
            {
                Name = name,
                Value = value
            };

            Suite.Mapper
                 .Setup(m => m.Map<ApplicationMappingServiceTestsSuite.Test>(dto))
                 .Returns(dest);

            var result = Suite.MappingService.Map<ApplicationMappingServiceTestsSuite.Test>(dto);

            Suite.Mapper
                 .Verify(m => m.Map<ApplicationMappingServiceTestsSuite.Test>(dto));

            Assert.Equal(name, dest.Name);
            Assert.Equal(value, dest.Value);
        }

        [Fact]
        public void MapWithExistObject()
        {
            var name = "Name";
            var value = 1;

            var dto = new ApplicationMappingServiceTestsSuite.TestDTO
            {
                Name = name,
                Value = value
            };

            var dest = new ApplicationMappingServiceTestsSuite.Test
            {
                Name = name,
                Value = value
            };

            Suite.Mapper
                 .Setup(m => m.Map<ApplicationMappingServiceTestsSuite.Test>(dto))
                 .Returns(dest);

            Suite.MappingService.Map(dto, dest);

            Suite.Mapper
                 .Verify(m => m.Map(dto, dest));

            Assert.Equal(name, dest.Name);
            Assert.Equal(value, dest.Value);
        }
    }
}
