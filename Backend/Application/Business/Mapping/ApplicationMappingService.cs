using AutoMapper;

using TransportSystems.Backend.Application.Interfaces.Mapping;

namespace TransportSystems.Backend.Application.Business.Mapping
{
    public class ApplicationMappingService : IMappingService
    {
        public ApplicationMappingService(IMapper mapper)
        {
            Mapper = mapper;
        }

        protected IMapper Mapper { get; }

        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }
    }
}