namespace wMapper;

[Serializable]
public class NotMappedException : MapperExceptionException
{
    public NotMappedException(Type from, Type to) : this(from, to, null!) { }
    public NotMappedException(Type from, Type to, string message) : this(from, to, message, null!) { }
    public NotMappedException(Type from, Type to, string message, Exception inner) : base(from, to, message, inner)
    {
    }
}
