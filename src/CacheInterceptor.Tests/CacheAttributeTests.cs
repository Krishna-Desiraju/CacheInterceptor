namespace CacheInterceptor.Tests
{
    using System;
    using System.Threading;

    using Castle.DynamicProxy;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The cache attribute tests.
    /// </summary>
    [TestClass]
    public class CacheAttributeTests
    {
        /// <summary>
        /// Gets or sets the bar proxy.
        /// </summary>
        public IBar BarInterfaceProxy { get; set; }

        /// <summary>
        /// Gets or sets the foo bar.
        /// </summary>
        public FooBar FooBarProxy { get; set; }

        /// <summary>
        /// The initialize proxy.
        /// </summary>
        [TestInitialize]
        public void InitializeProxy()
        {
            IBar bar = new Bar();
            var pg = new ProxyGenerator();
            this.BarInterfaceProxy = pg.CreateInterfaceProxyWithTarget(bar, new CacheInterceptor());
            this.FooBarProxy = pg.CreateClassProxy<FooBar>(new CacheInterceptor());
        }

        /// <summary>
        /// The interface proxy method with cache attribute should return cached object with cache duration instead of a new one.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void ShouldReturnCachedObjectWithCacheDurationInsteadOfANewOne()
        {
            var o1 = this.BarInterfaceProxy.GetFoo("test1");
            var o2 = this.BarInterfaceProxy.GetFoo("test1");
            o1.Should().BeSameAs(o2);
            o1.Value2.Should().Be("test1");

            var o3 = this.BarInterfaceProxy.GetFoo("test2");
            o3.Value2.Should().Be("test2");
        }

        /// <summary>
        /// The should return different values for different arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void ShouldReturnDifferentValuesForDifferentArguments()
        {
            var o1 = this.BarInterfaceProxy.GetFoo("test1");
            o1.Value2.Should().Be("test1");

            var o3 = this.BarInterfaceProxy.GetFoo("test2");
            o3.Value2.Should().Be("test2");
        }

        /// <summary>
        /// The should return different values for different arguments.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void ShouldBeAbleToCacheNullValuesAsWellWithOutException()
        {
            var o1 = this.BarInterfaceProxy.GetNullObject();
            var o2 = this.BarInterfaceProxy.GetNullObject();
            o1.Should().BeNull();
            o2.Should().BeNull();
        }

        /// <summary>
        /// The interface proxy method with cache attribute should return cached object with cache duration instead of a new one.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void ShouldReturnCachedObjectWithCacheDurationInsteadOfANewOneForClassProxy()
        {
            var o1 = this.FooBarProxy.GetFoo("test1");
            var o2 = this.FooBarProxy.GetFoo("test1");
            o1.Should().BeSameAs(o2);
            o1.Value2.Should().Be("test1");

            var o3 = this.FooBarProxy.GetFoo("test2");
            o3.Value2.Should().Be("test2");
        }

        /// <summary>
        /// The interface proxy method with cache attribute should return cached object with cache duration instead of a new one.
        /// </summary>
        [TestMethod]
        [TestCategory("Unit")]
        public void ShouldNotReturnCachedObjectAfterCacheDuration()
        {
            var o1 = this.BarInterfaceProxy.GetFoo("test1");
            Thread.Sleep(TimeSpan.FromSeconds(6));
            var o2 = this.BarInterfaceProxy.GetFoo("test1");

            o1.Should().NotBeSameAs(o2);
        }
    }
}
