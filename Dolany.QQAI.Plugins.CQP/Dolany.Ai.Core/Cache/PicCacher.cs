namespace Dolany.Ai.Core.Cache
{
    using System.Collections.Generic;

    using Common;

    using Dolany.Ai.Common;

    using Entities;

    public class PicCacher
    {
        private static readonly List<string> CacheList = new List<string>();

        private static readonly int MaxPicCacheCount = int.Parse(Configger.Instance["MaxPicCacheCount"]);

        private static readonly object RW_lock = new object();

        private static readonly object List_lock = new object();

        public static void Cache(string picUrl)
        {
            lock (List_lock)
            {
                if (CacheList.Contains(picUrl))
                {
                    return;
                }

                CacheList.Add(picUrl);
                if (CacheList.Count > MaxPicCacheCount)
                {
                    CacheList.RemoveAt(0);
                }
            }
        }

        public static void Save()
        {
            lock (RW_lock)
            {
                DbMgr.Delete<PicCacheEntity>(p => true);
                lock (List_lock)
                {
                    foreach (var cache in CacheList)
                    {
                        DbMgr.Insert(new PicCacheEntity { Content = cache });
                    }
                }
            }
        }

        public static void Load()
        {
            lock (RW_lock)
            {
                var query = DbMgr.Query<PicCacheEntity>();
                lock (List_lock)
                {
                    foreach (var entity in query)
                    {
                        CacheList.Add(entity.Content);
                    }
                }
            }
        }

        public static string Random()
        {
            lock (List_lock)
            {
                if (CacheList.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                var randIdx = Utility.RandInt(CacheList.Count);
                return CacheList[randIdx];
            }
        }
    }
}
