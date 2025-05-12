namespace WMapper;

[Serializable]
public class AlreadyMappedException : MapperExceptionException
{
    public AlreadyMappedException(Type from, Type to) : this(from, to, null!) { }
    public AlreadyMappedException(Type from, Type to, string message) : this(from, to, message, null!) { }
    public AlreadyMappedException(Type from, Type to, string message, Exception inner) : base(from, to, message, inner)
    {
    }
}
