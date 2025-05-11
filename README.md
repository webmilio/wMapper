# wMapper

wMapper is a very simple object-to-object mapper. It was created to be extremely plug-and-play, meaning one library, a few lines to add the service, and everything else is you defining your mappings. As of today, there is no auto-mapping.

Example usage: 
You can find a sample [here](wMapper.Samples/Program.cs).

The setup is straightforward. For any service-related usage, use the `services.AddMapper(options => ...)` extension method provided.
For example:
```csharp
var collection = new ServiceCollection();
collection.AddMapper(builder =>
{
    builder.Register<UserDbo, UserDto, UserMapper>();
});
```

This will add a singleton registration of type `IMapper` (implementation type of `Mapper`) and register the specified adapter (in this case `UserMapper`) for the specified conversion types (in this case `UserDbo -> UserDto`).

In other cases, it is preferable to use an instance of `IMapperBuilder`, registering your adapters and then building into an instance of `IMapper`. This package comes with the `MapperBuilder` class for this purpose.

### Adapters
There are two ways to register adapters. The first one is to use the generic overload provided by the `IMapperBuilder` interface that requires you to specify the type of the adapter.
```csharp
builder.Register<X, Y, XtoYAdapter>()
```

This will also handle any required dependency injection when creating `XtoYAdapter`.

The second way is to use the generic overload that requires an instance of `IAdapter<TFrom, TTo>`:
```csharp
builder.Register<X, Y>(new XtoYAdapter())
```

### Adapting
To actually use the `IMapper` in your code, you have the interface-provided `T Adapt<T>(object src, Type srcType);` method, where `srcType` is the type of the `src` parameter that was registered during configuration. The builder does not recognize childrens as valid registered types and will throw if you try to adapt to/from a non-registered type.

You can also use the extension method:
```csharp
mapper.Adapt<Y>(new X());
```
Be careful as this simply does a `.GetType()` on the provided `src` parameter. If you are adapting from a child of `X` to `Y`, you can use the second extension method:
```csharp
mapper.Adapt<X, Y>(new ChildofX());
```