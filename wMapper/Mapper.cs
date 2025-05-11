namespace wMapper;

public delegate object AdapterBoxer(object src);

public interface IMapper
{
    T Adapt<T>(object src, Type srcType);
}

public class Mapper(IDictionary<Type, IDictionary<Type, AdapterBoxer>> adapters) : IMapper
{
    public T Adapt<T>(object src, Type srcType)
    {
        var toType = typeof(T);
        if (!adapters.TryGetValue(srcType, out var fromAdpt))
        {
            throw new NotMappedException(srcType, typeof(T), $"No adapter registered for type from: {srcType} -> {toType}.");
        }

        if (!fromAdpt.TryGetValue(toType, out var toAdpt))
        {
            throw new NotMappedException(srcType, toType, $"No adapter registered for type to: {srcType} -> {toType}.");
        }

        return (T)toAdpt(src!);
    }
}

public static class IMapperExtensions
{
    public static T Adapt<T>(this IMapper mapper, object src)
    {
        return mapper.Adapt<T>(src, src.GetType());
    }

    public static TTo Adapt<TFrom, TTo>(this IMapper mapper, TFrom src) where TFrom : notnull
    {
        return mapper.Adapt<TTo>(src, typeof(TFrom));
    }
}