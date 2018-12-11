namespace Dolany.Ai.Core.Common
{
    using System;

    public static class Global
    {
        public static string AuthCode;
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
