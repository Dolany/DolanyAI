namespace Dolany.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CacheTable
    {
        public string Name { get; set; }

        private readonly object List_lock = new object();

        private readonly List<CacheModel> CacheList = new List<CacheModel>();

        public void Cache(string key, string value, DateTime? expireTime)
        {
            lock (List_lock)
            {
                var cache = this.CacheList.FirstOrDefault(c => c.Key == key);
                if (cache == null)
                {
                    this.CacheList.Add(new CacheModel { Key = key, Value = value, ExpireTime = expireTime });
                }
                else
                {
                    cache.Value = value;
                    cache.ExpireTime = expireTime;
                }
            }
        }

        public string Get(string key)
        {
            lock (List_lock)
            {
                var cache = this.CacheList.FirstOrDefault(c => c.Key == key);
                return cache == null ? string.Empty : cache.Value;
            }
        }

        public void Refresh()
        {
            lock (List_lock)
            {
                var now = DateTime.Now;
                this.CacheList.RemoveAll(c => c.ExpireTime != null && c.ExpireTime < now);
            }
        }
    }
}
