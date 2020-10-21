namespace Dolany.Ai.Common
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    /// <typeparam name="ConfigType"></typeparam>
    public class Configger<ConfigType> : IDataMgr
    {
        /// <summary>
        /// 配置实体
        /// </summary>
        public ConfigType AIConfig { get; set; }

        /// <summary>
        /// 配合实例
        /// </summary>
        public static Configger<ConfigType> Instance { get; } = new Configger<ConfigType>();

        private static DataRefreshSvc DataRefreshSvc => AutofacSvc.Resolve<DataRefreshSvc>();

        private Configger()
        {
            RefreshData();
            DataRefreshSvc.Register(this);
        }

        public void RefreshData()
        {
            AIConfig = CommonUtil.ReadJsonData<ConfigType>("AIConfigData");
        }
    }

    /// <summary>
    /// 默认配置实体
    /// </summary>
    public class AIConfigBase
    {
        public long DeveloperNumber { get; set; }

        public string CommandQueueName { get; set; }

        public string InformationQueueName { get; set; }

        public string CacheDb { get; set; }

        public bool IsTesting { get; set; }

        public long[] TestGroups { get; set; }

        public string Mutex { get; set; }

        public string FixedSetMutex { get; set; }

        public string FixedSetCacheDb { get; set; }

        public string MongoConnStr { get; set; }

        public string MongoDbName { get; set; }

        public string MainAi { get; set; }

        public string RedisHost { get; set; }

        public string RedisPort { get; set; }

        public string RedisPwd { get; set; }
    }
}
