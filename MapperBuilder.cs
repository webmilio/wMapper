using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using AdapterBoxer = wMapper.Mapper.AdapterBoxer;

namespace wMapper;

public interface IMapperBuilder
{
    IMapperBuilder Register<TAdapter, TFrom, TTo>() where TAdapter : IAdapter<TFrom, TTo>;

    IMapper Build();
}

public class MapperBuilder(IServiceProvider services) : IMapperBuilder
{
    private delegate AdapterBoxer AdapterBoxerProvider();

    private readonly ConcurrentDictionary<Type, object> _cache = [];
    private readonly Dictionary<Type, Dictionary<Type, AdapterBoxerProvider>> _boxerBuilders = [];

    public IMapper Build()
    {
        var built = new Dictionary<Type, IDictionary<Type, AdapterBoxer>>();

        foreach (var (from, builders) in _boxerBuilders)
        {
            var boxers = new Dictionary<Type, AdapterBoxer>();
            foreach (var (to, provider) in builders)
            {
                var boxer = provider();
                boxers.Add(to, boxer);
            }

            built.Add(from, boxers);
        }

        var mapper = ActivatorUtilities.CreateInstance<Mapper>(services, built);
        return mapper;
    }

    public IMapperBuilder Register<TAdapter, TFrom, TTo>() where TAdapter : IAdapter<TFrom, TTo>
    {
        if (!_boxerBuilders.TryGetValue(typeof(TFrom), out var builders))
        {
            _boxerBuilders.Add(typeof(TFrom), builders = []);
        }

        if (builders.ContainsKey(typeof(TTo)))
        {
            throw new AlreadyMappedException(typeof(TFrom), typeof(TTo), "There is already an adapter registered for the specified types.");
        }

        object Box(object value)
        {
            TAdapter adapter;

            if (_cache.TryGetValue(typeof(TAdapter), out var cachedAdapter))
            {
                adapter = (TAdapter)cachedAdapter;
            }
            else
            {
                adapter = ActivatorUtilities.CreateInstance<TAdapter>(services);
                _cache.TryAdd(typeof(TAdapter), adapter);
            }

            return adapter.Adapt((TFrom)value)!;
        }

        builders.Add(typeof(TTo), () => Box);
        return this;
    }
}