using System;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.Cache
{
    public class RapidCacher : IDependency
    {
        /// <summary>
        /// 读取/设置缓存
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="expirySpan"></param>
        /// <param name="refreshFunc"></param>
        /// <returns></returns>
        public static TResult GetCache<TResult>(string cacheKey, TimeSpan expirySpan, Func<TResult> refreshFunc)
        {
            try
            {
                var cache = RapidCacheRec.Get(cacheKey);
                if (!string.IsNullOrEmpty(cache))
                {
                    return JsonConvert.DeserializeObject<TResult>(cache);
                }

                var result = refreshFunc();
                if (result != null)
                {
                    RapidCacheRec.Set(cacheKey, JsonConvert.SerializeObject(result), expirySpan);
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return default;
            }
        }
    }

    public class RapidCacheRec : DbBaseEntity
    {
        public string CacheKey { get; set; }

        public string CacheValue { get;set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpiryTime { get; set; }

        public static string Get(string key)
        {
            return MongoService<RapidCacheRec>.GetOnly(p => p.CacheKey == key)?.CacheValue;
        }

        public static void Set(string key, string value, TimeSpan span)
        {
            var expiryTime = DateTime.Now + span;
            var filter = Builders<RapidCacheRec>.Filter.Where(p => p.CacheKey == key);
            var update = Builders<RapidCacheRec>.Update.Set(p => p.CacheValue, value).Set(p => p.ExpiryTime, expiryTime);
            MongoService<RapidCacheRec>.GetCollection().FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<RapidCacheRec>() {IsUpsert = true});
        }
    }
}
