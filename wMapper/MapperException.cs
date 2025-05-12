namespace WMapper;

[Serializable]
public class MapperExceptionException : Exception
{
    public MapperExceptionException(Type from, Type to) : this(from, to, null!) { }
    public MapperExceptionException(Type from, Type to, string message) : this(from, to, message, null!) { }
    public MapperExceptionException(Type from, Type to, string message, Exception inner) : base(message, inner)
    {
        From = from;
        To = to;
    }

    public Type From { get; }
    public Type To { get; }
}