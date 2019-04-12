namespace Dolany.Database.Redis
{
    using System;

    using Newtonsoft.Json;

    using StackExchange.Redis;

    public class Redis : ICache
    {
        private readonly JsonSerializerSettings jsonConfig = new JsonSerializerSettings()
                                                                 {
                                                                     ReferenceLoopHandling =
                                                                         ReferenceLoopHandling.Ignore,
                                                                     NullValueHandling = NullValueHandling.Ignore
                                                                 };

        private readonly IDatabase database;

        private sealed class CacheObject<T>
        {
            public int ExpireTime { get; set; }
            public bool ForceOutofDate { get; set; }
            public T Value { get; set; }
        }

        public Redis()
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1");
            database = connectionMultiplexer.GetDatabase();
        }

        /// <inheritdoc />
        /// <summary>
        /// 连接超时设置
        /// </summary>
        public int TimeOut { get; set; } = 600;

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public T Get<T>(string key)
        {
            var cacheValue = database.StringGet(key);
            var value = default(T);
            if (cacheValue.IsNull)
            {
                return value;
            }

            var cacheObject = JsonConvert.DeserializeObject<CacheObject<T>>(cacheValue, this.jsonConfig);
            if (!cacheObject.ForceOutofDate)
            {
                this.database.KeyExpire(key, new TimeSpan(0, 0, cacheObject.ExpireTime));
            }
            value = cacheObject.Value;

            return value;
        }

        public void Insert(string key, object data)
        {
            var jsonData = GetJsonData(data, TimeOut, false);
            database.StringSet(key, jsonData);
        }

        public void Insert(string key, object data, int cacheTime)
        {
            var timeSpan = TimeSpan.FromSeconds(cacheTime);
            var jsonData = GetJsonData(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }

        public void Insert(string key, object data, DateTime cacheTime)
        {
            var timeSpan = cacheTime - DateTime.Now;
            var jsonData = GetJsonData(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }

        public void Insert<T>(string key, T data)
        {
            var jsonData = GetJsonData(data, TimeOut, false);
            database.StringSet(key, jsonData);
        }

        public void Insert<T>(string key, T data, int cacheTime)
        {
            var timeSpan = TimeSpan.FromSeconds(cacheTime);
            var jsonData = GetJsonData(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }

        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            var timeSpan = cacheTime - DateTime.Now;
            var jsonData = GetJsonData(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }

        private string GetJsonData(object data, int cacheTime, bool forceOutOfDate)
        {
            var cacheObject =
                new CacheObject<object>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate };
            return JsonConvert.SerializeObject(cacheObject, jsonConfig); //序列化对象
        }

        private string GetJsonData<T>(T data, int cacheTime, bool forceOutOfDate)
        {
            var cacheObject =
                new CacheObject<T>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate };
            return JsonConvert.SerializeObject(cacheObject, jsonConfig); //序列化对象
        }

        /// <inheritdoc />
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            database.KeyDelete(key);
        }

        /// <inheritdoc />
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public bool Exists(string key)
        {
            return database.KeyExists(key);
        }
    }
}
