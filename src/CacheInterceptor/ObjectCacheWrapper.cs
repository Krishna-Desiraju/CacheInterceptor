namespace CacheInterceptor
{
    using System;
    using System.Runtime.Caching;

    /// <summary>
    /// The object cache wrapper.
    /// </summary>
    public static class ObjectCacheWrapper
    {
        private static readonly ObjectCache Cache = MemoryCache.Default;

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object Get(string key)
        {
            try
            {
                return Cache[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// The set.
        /// </summary>
        /// <param name="objectToCache">
        /// The object to cache.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="cacheTimeSpanInSeconds">
        /// The cache Time Span In Seconds.
        /// </param>
        public static void Set(object objectToCache, string key, long cacheTimeSpanInSeconds)
        {
            Cache.Add(key, objectToCache, DateTimeOffset.Now.AddSeconds(cacheTimeSpanInSeconds));
        }
    }
}