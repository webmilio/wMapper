namespace wMapper;

public interface IAdapter<TFrom, TTo>
{
    TTo Adapt(TFrom src);
}
