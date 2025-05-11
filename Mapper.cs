namespace wMapper;

public interface IMapper
{
    T Adapt<T>(object src, Type srcType);
}

public class Mapper(IDictionary<Type, IDictionary<Type, Mapper.AdapterBoxer>> boxers) : IMapper
{
    public delegate object AdapterBoxer(object src);

    public TTo Adapt<TFrom, TTo>(TFrom src)
    {
        if (!boxers.TryGetValue(typeof(TFrom), out var fromBoxers) ||
            !fromBoxers.TryGetValue(typeof(TTo), out var toBoxer))
        {
            throw new NotMappedException(typeof(TFrom), typeof(TTo), "No mapping registered for the specified types.");
        }

        return (TTo)toBoxer(src!);
    }

    public T Adapt<T>(object src, Type srcType)
    {
        if (!boxers.TryGetValue(srcType, out var fromAdpt) ||
            !fromAdpt.TryGetValue(typeof(T), out var toAdpt))
        {
            throw new NotMappedException(srcType, typeof(T), "No mapping registered for the specified types.");
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