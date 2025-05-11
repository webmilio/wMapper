using System.ComponentModel.Design;
using static wMapper.Tests.Classes;

namespace wMapper.Tests;

[TestClass]
public sealed class MapperBuilderTests
{
    private readonly IServiceProvider _services = new ServiceContainer();
    private readonly X _from;

    public MapperBuilderTests()
    {
        _from = new X()
        {
            Value = 5
        };
    }

    [TestMethod]
    public void Build_NoneRegistered_Throws()
    {
        var builder = new MapperBuilder(_services);
        var mapper = builder.Build();

        var value = new Random().Next();
        

        Assert.ThrowsException<NotMappedException>(() => mapper.Adapt<Y>(_from));
    }

    [TestMethod]
    public void Build_RegisterGeneric_Adapts()
    {
        var builder = new MapperBuilder(_services);
        builder.Register<X, Y, XtoYAdapter>();

        var mapper = builder.Build();
        var y = mapper.Adapt<Y>(_from);

        Assert.AreEqual($"{_from.Value}", y.Value);
    }

    [TestMethod]
    public void Build_RegisterSameFromToGeneric_Throws()
    {
        var builder = new MapperBuilder(_services);
        builder.Register<X, Y, XtoYAdapter>();

        Assert.ThrowsException<AlreadyMappedException>(builder.Register<X, Y, XtoYAdapter>);
    }

    [TestMethod]
    public void Build_RegisterSameFromToInstance_Throws()
    {
        var builder = new MapperBuilder(_services);
        builder.Register(new XtoYAdapter());

        Assert.ThrowsException<AlreadyMappedException>(() => builder.Register(new XtoYAdapter()));
    }

    [TestMethod]
    public void Build_RegisterInstance_Adapts()
    {
        var builder = new MapperBuilder(_services);
        builder.Register(new XtoYAdapter());

        var mapper = builder.Build();
        var y = mapper.Adapt<Y>(_from);

        Assert.AreEqual($"{_from.Value}", y.Value);
    }

    [TestMethod]
    public void Build_GenericAdapts_Multiple()
    {
        var builder = new MapperBuilder(_services);
        builder.Register<X, Y, XtoYAdapter>();

        var mapper = builder.Build();
        
        var xs = new X[100];
        Array.Fill(xs, _from);

        var ys = new Y[xs.Length];

        for (int i = 0; i < xs.Length; i++)
        {
            ys[i] = mapper.Adapt<Y>(xs[i]);
        }

        foreach (var y in ys)
        {
            Assert.AreEqual($"{_from.Value}", y.Value);
        }
    }
}
