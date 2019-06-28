namespace Dolany.Ai.Common
{
    public class Configger
    {
        public AIConfig AIConfig { get; }

        public static Configger Instance { get; } = new Configger();

        private Configger()
        {
            AIConfig = CommonUtil.ReadJsonData<AIConfig>("AIConfigData");
        }
    }

    public class AIConfig
    {
        public int PraiseLimit { get; set; }

        public int PicCleanFreq { get; set; }

        public int MaxPicCacheCount { get; set; }

        public int MaxOriginPicCache { get; set; }

        public string TulingRequestUrl { get; set; }

        public long DeveloperNumber { get; set; }

        public long SysMsgNumber { get; set; }

        public int DiceCountMaxLimit { get; set; }

        public int DiceSizeMaxLimit { get; set; }

        public int MaxRecentCommandCacheCount { get; set; }

        public string InformationQueueName { get; set; }

        public string CacheDb { get; set; }

        public bool IsTesting { get; set; }

        public long[] TestGroups { get; set; }

        public long AnonymousNumber { get; set; }

        public string Mutex { get; set; }

        public string FixedSetMutex { get; set; }

        public string FixedSetCacheDb { get; set; }

        public string MongoConnStr { get; set; }

        public string MongoDbName { get; set; }

        public string MainAi { get; set; }
    }
}
