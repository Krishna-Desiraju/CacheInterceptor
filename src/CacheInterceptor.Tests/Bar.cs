namespace CacheInterceptor.Tests
{
    /// <summary>
    /// The bar.
    /// </summary>
    public class Bar : IBar
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
        public Foo GetFoo(string value)
        {
            return new Foo(value);
        }

        /// <summary>
        /// The get null object.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetNullObject()
        {
            return null;
        }
    }
}