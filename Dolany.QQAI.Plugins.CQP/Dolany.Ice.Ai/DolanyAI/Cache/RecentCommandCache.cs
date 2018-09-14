using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class RecentCommandCache
    {
        private static int MaxRecentCommandCacheCount;

        private static List<DateTime> TimeCache;

        public static void Cache(DateTime time)
        {
            if (TimeCache == null)
            {
                Init();
            }

            Debug.Assert(TimeCache != null, nameof(TimeCache) + " != null");
            if (TimeCache.Count > MaxRecentCommandCacheCount)
            {
                TimeCache.RemoveAt(0);
            }
        }

        private static void Init()
        {
            TimeCache = new List<DateTime>();
            MaxRecentCommandCacheCount = int.Parse(Utility.GetConfig(nameof(MaxRecentCommandCacheCount)));
        }

        public static bool IsTooFreq()
        {
            if (TimeCache == null)
            {
                Init();
            }

            Debug.Assert(TimeCache != null, nameof(TimeCache) + " != null");
            if (!TimeCache.Any())
            {
                return false;
            }

            if (TimeCache.First().AddMinutes(1) < TimeCache.Last())
            {
                return true;
            }

            return false;
        }
    }
}