using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Common
{
    public class Global
    {
        public static bool IsTesting { get; } = bool.Parse(Configger.Instance["IsTesting"]);

        public static long DeveloperNumber { get; } = long.Parse(Configger.Instance["DeveloperNumber"]);

        public static long SysMsgNumber { get; } = long.Parse(Configger.Instance["SysMsgNumber"]);

        public static long AnonymousNumber { get; } = long.Parse(Configger.Instance["AnonymousNumber"]);

        public static IEnumerable<long> TestGroups { get; } = Configger.Instance["TestGroups"].Split(" ").Select(long.Parse);

        public static Dictionary<long, string> AllGroupsDic => GroupSettingMgr.Instance.SettingDic.ToDictionary(p => p.Key, p => p.Value.Name);

        public static readonly RabbitMQService CommandInfoService =
            new RabbitMQService(Configger.Instance["InformationQueueName"]);
    }

    public enum MsgType
    {
        Private = 1,
        Group = 0
    }
}
