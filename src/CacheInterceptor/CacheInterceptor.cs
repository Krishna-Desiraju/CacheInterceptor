namespace CacheInterceptor
{
    using System;
    using System.Linq;
    using System.Text;

    using Castle.DynamicProxy;

    /// <summary>
    /// The cache interceptor.
    /// </summary>
    public class CacheInterceptor : IInterceptor
    {
        /// <summary>
        /// Intercepts the DAO calls to reference tables and caches them to minimize DB round trips
        /// </summary>
        /// <param name="invocation">
        /// The invocation.
        /// </param>
        public void Intercept(IInvocation invocation)
        {
            // if the interface is decorated with cache attribute
            var cacheDecorators = invocation.Method.GetCustomAttributes(typeof(CacheAttribute), true);
            if (cacheDecorators.Any())
            {
                var keyForCache = CreateInvocationKeyString(invocation);
                var cachedReturnValue = ObjectCacheWrapper.Get(keyForCache);
                if (cachedReturnValue != null)
                {
                    invocation.ReturnValue = cachedReturnValue == DBNull.Value ? null : cachedReturnValue; // since nulls can't cached - they are converted to dummy values
                    return;
                }

                invocation.Proceed();

                // add to cache
                var objectToCache = invocation.ReturnValue ?? DBNull.Value;
                var propInfo = cacheDecorators[0].GetType().GetProperty("CacheTimeSpanInSeconds");
                var cacheTimeSpanInSeconds = (long)propInfo.GetValue(cacheDecorators[0], null);
                ObjectCacheWrapper.Set(objectToCache, keyForCache, cacheTimeSpanInSeconds);
            }
            else
            {
                invocation.Proceed();
            }
        }

        private static string CreateInvocationKeyString(IInvocation invocation)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}.{1}(", invocation.TargetType.FullName, invocation.Method.Name);
            if (invocation.Arguments.Any())
            {
                sb.Append(invocation.Arguments.Select(UtilityExtensions.DumpObject).Aggregate((x, y) => x + "," + y));
            }

            sb.Append(")");
            return sb.ToString();
        }
    }
}
