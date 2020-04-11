using System;
using System.Collections.Concurrent;
using Dolany.Ai.Common;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Common
{
    public class AliveStateSvc : IDependency
    {
        private readonly ConcurrentDictionary<string, AliveStateCache> CacheDic;

        public AliveStateSvc()
        {
            CacheDic = new ConcurrentDictionary<string, AliveStateCache>();
            var allCaches = MongoService<AliveStateCache>.Get();
            foreach (var Cache in allCaches)
            {
                CacheDic.TryAdd(Cache.Key, Cache);
            }
        }

        public AliveStateCache GetState(long GroupNum, long QQNum)
        {
            var key = $"{GroupNum}-{QQNum}";
            if (!CacheDic.ContainsKey(key))
            {
                return null;
            }

            var cache = CacheDic[key];
            if (cache.RebornTime >= DateTime.Now)
            {
                return cache;
            }

            cache.Delete();
            CacheDic.TryRemove(key, out _);
            return null;
        }

        public void Cache(AliveStateCache cache)
        {
            if (CacheDic.ContainsKey(cache.Key))
            {
                CacheDic[cache.Key] = cache;
                return;
            }

            cache.Insert();
            CacheDic.TryAdd(cache.Key, cache);
        }
    }

    public class AliveStateCache : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RebornTime { get; set; }

        public string Name { get; set; }

        public string Key => $"{GroupNum}-{QQNum}";

        public void Delete()
        {
            MongoService<AliveStateCache>.Delete(this);
        }

        public void Insert()
        {
            MongoService<AliveStateCache>.Insert(this);
        }
    }
}
