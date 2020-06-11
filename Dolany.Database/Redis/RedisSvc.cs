using System;
using Dolany.Ai.Common;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Dolany.Database.Redis
{
    public class RedisSvc
    {
        public static RedisSvc Instance { get; } = new RedisSvc();

        private ConnectionMultiplexer Redis;
        private IDatabase RedisDB;

        //静态类中使用该服务没有注入 手动传入相关引用
        private RedisSvc()
        {
            Connect();
        }

        /// <summary>
        /// 初始化连接
        /// </summary>
        private void Connect()
        {
            var option = new ConfigurationOptions();
            var config = Configger<AIConfigBase>.Instance.AIConfig;
            option.EndPoints.Add($"{config.RedisHost}:{config.RedisPort}");
            option.Password = config.RedisPwd;
            option.ConnectTimeout = 10 * 1000;
            option.SyncTimeout = 10 * 1000;
            Redis = ConnectionMultiplexer.Connect(option);
            RedisDB = Redis.GetDatabase();
        }

        public void Cache(string key, object value, TimeSpan? span = null)
        {
            var jsonStr = JsonConvert.SerializeObject(value);
            RedisDB.StringSet(key, jsonStr, span);
        }

        public T GetCache<T>(string key)
        {
            string value = RedisDB.StringGet(key);
            return string.IsNullOrEmpty(value) ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public bool TryLock(string key, TimeSpan lockSpan)
        {
            return RedisDB.LockTake(key, key, lockSpan);
        }

        public void ReleaseLock(string key)
        {
            RedisDB.LockRelease(key, key);
        }
    }
}
