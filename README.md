# CacheInterceptor

An AOP way to easily add cache layer to your application without modifying the core architecture of your application. This is for caching data access using method calls that are fairly constant for a known period of time. In my case, I developed it to reduce the number of roundtrips to the database when accessing reference tables for which data is static for certain limited time. And it can used in several instances like generating security tokens which are valid for a set expiration time.
As said earlier, it is easy to cache layer to your instance method without actually changing the code related to it. All that is required is to decorate the method with Cache attribute specifying the expiration of cache. This leverages castle core dynamic proxy and object cache.

### NuGet Packages

```
Install-Package CacheInterceptor
```

###Cache for Concrete Class Instance Methods
Lets see how we can add cache to an existing instance method

```csharp
public class FooBar
{
    public Foo GetFoo(string value)
    {
        return new Foo(value);
    }
}
```
in order to add cache layer, modify the method as follows
```csharp
using CacheInterceptor;

public class FooBar
{
    [Cache(5)]
    public virtual Foo GetFoo(string value)
    {
        return new Foo(value);
    }
}
```
here, the changes are, firstly, the method is decorated with Cache attribute specifying the expiration time in seconds, here it cache expiration time is 5 seconds. And the second change is make your instance method virtual.
And the final change is to invoke the instance method by creating a proxy using castle core dynamic proxy

and this how the invokation of the method should be modified
```csharp
var foobar = new FooBar();
var pg = new ProxyGenerator();
this.FooBarProxy = pg.CreateClassProxyWithTarget<FooBar>(foobar, new CacheInterceptor());

var o1 = this.FooBarProxy.GetFoo("test1");
var o2 = this.FooBarProxy.GetFoo("test1");
// here o1 == o2 -> these references point to the same object

Thread.Sleep(TimeSpan.FromSeconds(6));
var o3 = this.FooBarProxy.GetFoo("test1");
// o3 here is fetched from method and not from cache
var o4 = this.FooBarProxy.GetFoo("test1");
// again 03 == o4
```
###Cache for interface implementations
Decorate the interface declaration of the method and decorate it with cache
```csharp
    public interface IBar
    {
        [Cache(5)]
        Foo GetFoo(string value);
    }
```
Please note that for implemetation of cache of interfaces, the actual implementation class need not be decorated.
and change the invokation of the method as follows
```csharp
var pg = new ProxyGenerator();
IBar barInterfaceProxy = pg.CreateInterfaceProxyWithTarget(bar, new CacheInterceptor());
var o1 = barInterfaceProxy.GetFoo("test1");
```
and if you are using any DI/IoC container, enrich them with the interceptor. Here I am using structuremap
```csharp
var pg = new ProxyGenerator();
ObjectFactory.Container.Configure
    .For<IBar>()
    .EnrichAllWith(x => pg.CreateInterfaceProxyWithTarget(x, new CacheInterceptor()))
    .Use<Bar>();
```

Similarly, if you to cache for 12 hours, Cache attribute can be set as follows
```csharp
[Cache(60 * 60 * 12)]
// or
[Cache(43200)]
```

Other points to note:
* The key for cache object is Method Fullname with namespace and concatination of arguments.
* For arguments with primitive types, ToString() is used and for class types, data contract serialization is used. So if you are using class types as argument please ensure that they can be serialized using data contract serialization.
* Also caches null values
* Only supports instance methods and doesn't support static methods


