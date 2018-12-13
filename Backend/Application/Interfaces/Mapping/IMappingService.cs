namespace TransportSystems.Backend.Application.Interfaces.Mapping
{
    public interface IMappingService : IApplicationBaseService
    {
        T Map<T>(object source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }
}