using TransportSystems.Backend.Application.Business.Mapping;
using TransportSystems.Backend.Application.Interfaces.Mapping;

namespace TransportSystems.Backend.Application.Business.Tests.Suite
{
    public abstract class MappingTestsSuite : BaseTestsSuite
    {
        protected MappingTestsSuite()
        {
            var mapper = new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new ApplicationMappingProfile());
                }).CreateMapper();

            MappingService = new ApplicationMappingService(mapper);
        }

        public IMappingService MappingService { get; }
    }
}