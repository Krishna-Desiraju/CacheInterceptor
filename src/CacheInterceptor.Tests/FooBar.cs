namespace CacheInterceptor.Tests
{
    /// <summary>
    /// The bar.
    /// </summary>
    public class FooBar
    {
        /// <summary>
        /// The get foo.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Foo"/>.
        /// </returns>
        [Cache(5)]
        public virtual Foo GetFoo(string value)
        {
            return new Foo(value);
        }

        /// <summary>
        /// The get null object.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [Cache(5)]
        public virtual object GetNullObject()
        {
            return null;
        }
    }
}