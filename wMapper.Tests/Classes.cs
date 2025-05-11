namespace wMapper.Tests;

public class Classes
{
    public class X
    {
        public required int Value { get; set; }
    }

    public class Y
    {
        public required string Value { get; set; }
    }

    public class XtoYAdapter : IAdapter<X, Y>
    {
        public Y Adapt(X from)
        {
            return new()
            { 
                Value = from.Value.ToString() 
            };
        }
    }
}
