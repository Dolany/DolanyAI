using System;
using System.Collections.Concurrent;
using Dolany.Ai.Core.Common;
using Dolany.Database.Redis;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.Cache
{
    /// <summary>
    /// 快速缓存器
    /// </summary>
    public class RapidCacher
    {
        private static readonly ConcurrentDictionary<string, CacheModel> CachedDic = new ConcurrentDictionary<string, CacheModel>();
    
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
                CachedDic.TryGetValue(cacheKey, out var cache);
                if (cache != null && cache.IsInTime)
                {
                    return JsonConvert.DeserializeObject<TResult>(cache.ValueJson);
                }

                var result = refreshFunc();
                if (result == null)
                {
                    return null;
                }
                
                CachedDic.TryRemove(cacheKey, out _);
                CachedDic.TryAdd(cacheKey,
                                 new CacheModel()
                                 {
                                     Key        = cacheKey,
                                     ValueJson  = JsonConvert.SerializeObject(result),
                                     ExpiryTime = DateTime.Now.Add(expirySpan)
                                 });

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

    /// <summary>
    /// 缓存模型
    /// </summary>
    public class CacheModel
    {
        /// <summary>
        /// 缓存键
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// 缓存值（序列化后）
        /// </summary>
        public string ValueJson { get; set; }
        
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpiryTime { get; set; }

        /// <summary>
        /// 是否在有效期内
        /// </summary>
        public bool IsInTime => ExpiryTime == null || ExpiryTime > new DateTime();
    }
}
