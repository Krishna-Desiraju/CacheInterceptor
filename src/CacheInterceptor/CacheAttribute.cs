namespace CacheInterceptor
{
    using System;

    /// <summary>
    /// The cache attribute used as marker attribute to cache references
    /// </summary>
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheAttribute"/> class.
        /// </summary>
        /// <param name="cacheTimeSpanInSeconds">
        /// The cache time span in seconds.
        /// </param>
        public CacheAttribute(long cacheTimeSpanInSeconds)
        {
            this.CacheTimeSpanInSeconds = cacheTimeSpanInSeconds;
        }

        /// <summary>
        /// Gets the cache time span in seconds.
        /// </summary>
        public long CacheTimeSpanInSeconds { get; private set; }
    }
}
