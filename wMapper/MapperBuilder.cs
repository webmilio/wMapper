using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace wMapper;

public interface IMapperBuilder
{
    IMapperBuilder Register<TFrom, TTo>(IAdapter<TFrom, TTo> adapter);

    IMapperBuilder Register<TFrom, TTo, TAdapter>() where TAdapter : IAdapter<TFrom, TTo>;

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

    private IDictionary<Type, AdapterBoxerProvider> ValidateAndGetFromBuilders<TFrom, TTo>()
    {
        if (!_boxerBuilders.TryGetValue(typeof(TFrom), out var builders))
        {
            _boxerBuilders.Add(typeof(TFrom), builders = []);
        }

        if (builders.ContainsKey(typeof(TTo)))
        {
            throw new AlreadyMappedException(typeof(TFrom), typeof(TTo), $"There is already an adapter registered for types {typeof(TFrom)} -> {typeof(TTo)}.");
        }

        return builders;
    }

    public IMapperBuilder Register<TFrom, TTo>(Func<IAdapter<TFrom, TTo>> adapterProvider)
    {
        var builders = ValidateAndGetFromBuilders<TFrom, TTo>();

        object Box(object value)
        {
            var adapter = adapterProvider();
            return adapter.Adapt((TFrom)value)!;
        }

        builders.Add(typeof(TTo), () => Box);
        return this;
    }

    public IMapperBuilder Register<TFrom, TTo>(IAdapter<TFrom, TTo> adapter)
    {
        return Register(() => adapter);
    }

    public IMapperBuilder Register<TFrom, TTo, TAdapter>() where TAdapter : IAdapter<TFrom, TTo>
    {
        IAdapter<TFrom, TTo>? adapter = null;
        IAdapter<TFrom, TTo> GetOrMakeAdapter()
        {
            if (adapter == null)
            {
                if (_cache.TryGetValue(typeof(TAdapter), out var cachedAdapter))
                {
                    adapter = (TAdapter)cachedAdapter;
                }
                else
                {
                    adapter = ActivatorUtilities.CreateInstance<TAdapter>(services);
                    _cache.TryAdd(typeof(TAdapter), adapter);
                }
            }

            return adapter;
        }

        Register(GetOrMakeAdapter);
        return this;
    }
}