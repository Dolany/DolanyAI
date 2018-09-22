﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Dolany.Ice.Ai.DolanyAI.Utils.Utility;

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
            TimeCache.Add(time);
            if (TimeCache.Count > MaxRecentCommandCacheCount)
            {
                TimeCache.RemoveAt(0);
            }
        }

        private static void Init()
        {
            TimeCache = new List<DateTime>();
            MaxRecentCommandCacheCount = int.Parse(GetConfig(nameof(MaxRecentCommandCacheCount)));
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

            if (TimeCache.Count >= MaxRecentCommandCacheCount &&
                TimeCache.First().AddMinutes(1) > DateTime.Now)
            {
                return true;
            }

            return false;
        }
    }
}