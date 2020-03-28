using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Doremi.Common
{
    public class Global
    {
        public static bool IsTesting { get; } = Configger<AIConfigBase>.Instance.AIConfig.IsTesting;

        public static long DeveloperNumber { get; } = Configger<AIConfigBase>.Instance.AIConfig.DeveloperNumber;

        public static long SysMsgNumber { get; } = 10000;

        public static long AnonymousNumber { get; } = 80000000;

        public static IEnumerable<long> TestGroups { get; } = Configger<AIConfigBase>.Instance.AIConfig.TestGroups;

        public static Dictionary<long, string> AllGroupsDic => AutofacSvc.Resolve<GroupSettingSvc>().SettingDic.ToDictionary(p => p.Key, p => p.Value.Name);

        public static readonly RabbitMQService CommandInfoService =
            new RabbitMQService(Configger<AIConfigBase>.Instance.AIConfig.InformationQueueName);
    }
}
