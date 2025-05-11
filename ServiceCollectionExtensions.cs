using Microsoft.Extensions.DependencyInjection;

namespace wMapper;

public static class ServiceCollectionExtensions
{
    public static void AddMapper(this IServiceCollection services, Action<IMapperBuilder> configuration)
    {
        services.AddSingleton(s =>
        {
            var builder = ActivatorUtilities.CreateInstance<MapperBuilder>(s);
            configuration(builder);

            var mapper = builder.Build();
            return mapper;
        });
    }
}
