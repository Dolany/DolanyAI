using System;
using Dolany.Ai.Core.Common;
using Dolany.Database.Redis;

namespace Dolany.Ai.Core.Cache
{
    public class RapidCacher
    {
        /// <summary>
        /// 读取/设置缓存
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="expirySpan"></param>
        /// <param name="refreshFunc"></param>
        /// <returns></returns>
        public static TResult GetCache<TResult>(string cacheKey, TimeSpan expirySpan, Func<TResult> refreshFunc) where TResult:class
        {
            try
            {
                var cache = RedisSvc.Instance.GetCache<TResult>(cacheKey);
                if (cache != null)
                {
                    return cache;
                }

                var result = refreshFunc();
                if (result != null)
                {
                    RedisSvc.Instance.Cache(cacheKey, result, expirySpan);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return default;
            }
        }

        public static TResult GetCache<TResult>(string cacheKey, DateTime expiryTime, Func<TResult> refreshFunc) where TResult : class
        {
            return GetCache(cacheKey, expiryTime - DateTime.Now, refreshFunc);
        }

        public static void SetCache(string cacheKey, object cacheValue, TimeSpan expirySpan)
        {
            RedisSvc.Instance.Cache(cacheKey, cacheValue, expirySpan);
        }

        public static void SetCache(string cacheKey, object cacheValue, DateTime expiryTime)
        {
            RedisSvc.Instance.Cache(cacheKey, cacheValue, expiryTime - DateTime.Now);
        }
    }
}
