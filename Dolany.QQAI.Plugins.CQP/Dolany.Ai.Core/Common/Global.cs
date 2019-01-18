namespace Dolany.Ai.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Database;

    using Dolany.Ai.Common;

    public static class Global
    {
        public static string AuthCode { get; set; }

        public static bool IsTesting { get; } = bool.Parse(Configger.Instance["IsTesting"]);

        public static IEnumerable<long> TestGroups { get; } = Configger.Instance["TestGroups"].Split(" ").Select(long.Parse);

        public static readonly RabbitMQService CommandInfoService =
            new RabbitMQService(Configger.Instance["InformationQueueName"]);

        public static readonly RabbitMQService CacheInfoService =
            new RabbitMQService(Configger.Instance["CacheResponse"]);

        public static readonly long[] AllGroups =
            {
                39778531, 213486468, 948676729, 547176147, 158109222, 885795232, 397227892, 458243816, 276179056,
                922541845, 709460597, 961791187, 273170805, 957668528, 472634946, 469652754, 211922134, 517995259,
                451197900, 367797407, 423971201, 325210445, 239896071, 626351509
            };
    }

    public class Sys_ErrorCount
    {
        private static readonly object _lockObj = new object();

        private static int _count;

        public static int Get()
        {
            lock (_lockObj)
            {
                return _count;
            }
        }

        public static void Plus()
        {
            lock (_lockObj)
            {
                _count++;
            }
        }
    }

    public class Sys_CommandCount
    {
        private static readonly object _lockObj = new object();

        private static int _count;

        public static int Get()
        {
            lock (_lockObj)
            {
                return _count;
            }
        }

        public static void Plus()
        {
            lock (_lockObj)
            {
                _count++;
            }
        }

        public static void Minus()
        {
            lock (_lockObj)
            {
                _count--;
            }
        }
    }

    public class Sys_StartTime
    {
        private static DateTime _time;

        public static DateTime Get()
        {
            return _time;
        }

        public static void Set(DateTime time)
        {
            _time = time;
        }
    }

    public enum SysStatus
    {
        Count,
        StartTime
    }

    public enum MsgType
    {
        Private = 1,
        Group = 0
    }
}
