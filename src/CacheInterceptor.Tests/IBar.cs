namespace CacheInterceptor.Tests
{
    /// <summary>
    /// The Bar interface.
    /// </summary>
    public interface IBar
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
        Foo GetFoo(string value);

        /// <summary>
        /// The get null object.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [Cache(5)]
        object GetNullObject();
    }
}
