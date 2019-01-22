namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dolany.Ai.Common;

    public class RecentCommandCache
    {
        private static readonly int MaxRecentCommandCacheCount = int.Parse(Configger.Instance["MaxRecentCommandCacheCount"]);

        private static readonly List<DateTime> TimeCache = new List<DateTime>();

        private static readonly object Lock_list = new object();

        public static void Cache()
        {
            lock (Lock_list)
            {
                TimeCache.Add(DateTime.Now);
                if (TimeCache.Count > MaxRecentCommandCacheCount)
                {
                    TimeCache.RemoveAt(0);
                }
            }
        }

        public static bool IsTooFreq()
        {
            lock (Lock_list)
            {
                if (!TimeCache.Any())
                {
                    return false;
                }

                if (TimeCache.Count >= MaxRecentCommandCacheCount && TimeCache.First().AddMinutes(1) > DateTime.Now)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
