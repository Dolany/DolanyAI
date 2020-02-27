using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Common
{
    public class Global
    {
        public static bool IsTesting { get; } = DefaultConfig.IsTesting;

        public static long DeveloperNumber { get; } = DefaultConfig.DeveloperNumber;

        public static long SysMsgNumber { get; } = DefaultConfig.SysMsgNumber;

        public static long AnonymousNumber { get; } = DefaultConfig.AnonymousNumber;

        public static IEnumerable<long> TestGroups { get; } = DefaultConfig.TestGroups;

        public static AIConfigBase DefaultConfig => Configger<AIConfigBase>.Instance.AIConfig;

        public static Dictionary<long, string> AllGroupsDic => AutofacSvc.Resolve<GroupSettingSvc>().SettingDic.ToDictionary(p => p.Key, p => p.Value.Name);

        public static readonly RabbitMQService CommandInfoService = new RabbitMQService(DefaultConfig.InformationQueueName);

        public static Action<string> MsgPublish;
    }
}
