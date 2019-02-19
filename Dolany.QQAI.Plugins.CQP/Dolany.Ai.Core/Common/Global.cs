﻿namespace Dolany.Ai.Core.Common
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
    }

    public class Sys_ErrorCount
    {
        private static readonly object _lockObj = new object();

        private static readonly List<string> ErrorList = new List<string>();

        public static int GetCount()
        {
            lock (_lockObj)
            {
                return ErrorList.Count;
            }
        }

        public static void Plus(string msg)
        {
            lock (_lockObj)
            {
                ErrorList.Add(msg);
            }
        }

        public static string GetMsg(int index)
        {
            lock (_lockObj)
            {
                if (index < 0 || index >= ErrorList.Count)
                {
                    return string.Empty;
                }

                return ErrorList[index];
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
