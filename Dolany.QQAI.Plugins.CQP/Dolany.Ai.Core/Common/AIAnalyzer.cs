using System;
using System.Collections.Generic;

namespace Dolany.Ai.Core.Common
{
    public class AIAnalyzer
    {
        public static readonly List<int> HourlyCommandCount = new List<int>();

        private static readonly object Lock = new object();

        public static DateTime Sys_StartTime;

        private static int Sys_CommandCount;

        private static readonly List<string> ErrorList = new List<string>();

        public const int MaxCommandRecordCount = 12;

        public static void CountRecord(int count)
        {
            HourlyCommandCount.Add(count);
            if (HourlyCommandCount.Count > MaxCommandRecordCount)
            {
                HourlyCommandCount.RemoveAt(0);
            }
        }

        public static void AddCommandCount()
        {
            lock (Lock)
            {
                Sys_CommandCount++;
            }
        }

        public static int GetCommandCount()
        {
            lock (Lock)
            {
                return Sys_CommandCount;
            }
        }

        public static int GetErrorCount()
        {
            lock (Lock)
            {
                return ErrorList.Count;
            }
        }

        public static void AddError(string msg)
        {
            lock (Lock)
            {
                ErrorList.Add(msg);
            }
        }

        public static string GetErrorMsg(int index)
        {
            lock (Lock)
            {
                if (index < 0 || index >= ErrorList.Count)
                {
                    return string.Empty;
                }

                return ErrorList[index];
            }
        }
    }
}
