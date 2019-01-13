namespace Dolany.Database.Redis
{
    using System;

    /// <summary>
    /// 缓存
    /// </summary>
    public static class CacheService
    {
        private static readonly object cacheLocker = new object(); //缓存锁对象

        private static ICache cache; //缓存接口

        static CacheService()
        {
            Load();
        }

        /// <summary>
        /// 加载缓存
        /// </summary>
        private static void Load()
        {
            lock (cacheLocker)
            {
                cache = new Redis();
            }
        }

        public static ICache GetCache()
        {
            return cache;
        }

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public static int TimeOut
        {
            get => cache.TimeOut;
            set
            {
                lock (cacheLocker)
                {
                    cache.TimeOut = value;
                }
            }
        }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public static T Get<T>(string key)
        {
            return cache.Get<T>(key);
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        public static void Insert<T>(string key, T data)
        {
            if (string.IsNullOrWhiteSpace(key) || data == null)
            {
                return;
            }

            lock (cacheLocker)
            {
                cache.Insert(key, data);
            }
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间(分钟)</param>
        public static void Insert<T>(string key, T data, int cacheTime)
        {
            if (string.IsNullOrWhiteSpace(key) || data == null)
            {
                return;
            }

            lock (cacheLocker)
            {
                cache.Insert(key, data, cacheTime);
            }
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        public static void Insert<T>(string key, T data, DateTime cacheTime)
        {
            if (string.IsNullOrWhiteSpace(key) || data == null)
            {
                return;
            }

            lock (cacheLocker)
            {
                cache.Insert(key, data, cacheTime);
            }
        }

        /// <summary>
        /// 从缓存中移除指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        public static void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }
            lock (cacheLocker)
            {
                cache.Remove(key);
            }
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Exists(string key)
        {
            lock (cacheLocker)
            {
                return cache.Exists(key);
            }
        }
    }
}
