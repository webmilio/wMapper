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
        if (!adapters.TryGetValue(srcType, out var fromAdpt) ||
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