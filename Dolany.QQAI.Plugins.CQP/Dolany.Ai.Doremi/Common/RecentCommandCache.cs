using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolany.Ai.Doremi.Common
{
    public class RecentCommandCache
    {
        private const int MaxRecentCommandCacheCount = 130;

        private static readonly Dictionary<string, List<DateTime>> TimeCacheDic = new Dictionary<string, List<DateTime>>();

        private static readonly object Lock_list = new object();

        public static void Cache(string BindAi)
        {
            lock (Lock_list)
            {
                if (TimeCacheDic.ContainsKey(BindAi))
                {
                    var tc = TimeCacheDic[BindAi];
                    tc.Add(DateTime.Now);
                    if (tc.Count > MaxRecentCommandCacheCount)
                    {
                        tc.RemoveAt(0);
                    }
                }
                else
                {
                    TimeCacheDic.Add(BindAi, new List<DateTime>(){DateTime.Now});
                }
            }
        }

        public static bool IsTooFreq(string BindAi)
        {
            lock (Lock_list)
            {
                if (!TimeCacheDic.ContainsKey(BindAi) || !TimeCacheDic[BindAi].Any())
                {
                    return false;
                }

                return TimeCacheDic[BindAi].Count >= MaxRecentCommandCacheCount && TimeCacheDic[BindAi].First().AddHours(1) > DateTime.Now;
            }
        }
    }
}
