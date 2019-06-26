using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Common
{
    public class Global
    {
        public static bool IsTesting { get; } = Configger.Instance.AIConfig.IsTesting;

        public static long DeveloperNumber { get; } = Configger.Instance.AIConfig.DeveloperNumber;

        public static long SysMsgNumber { get; } = Configger.Instance.AIConfig.SysMsgNumber;

        public static long AnonymousNumber { get; } = Configger.Instance.AIConfig.AnonymousNumber;

        public static IEnumerable<long> TestGroups { get; } = Configger.Instance.AIConfig.TestGroups;

        public static Dictionary<long, string> AllGroupsDic => GroupSettingMgr.Instance.SettingDic.ToDictionary(p => p.Key, p => p.Value.Name);

        public static readonly RabbitMQService CommandInfoService =
            new RabbitMQService(Configger.Instance.AIConfig.InformationQueueName);
    }

    public enum MsgType
    {
        Private = 1,
        Group = 0
    }
}
