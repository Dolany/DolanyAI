namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dolany.Ai.Common;

    public class RecentCommandCache
    {
        private static readonly int MaxRecentCommandCacheCount = int.Parse(Configger.Instance["MaxRecentCommandCacheCount"]);

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

                if (TimeCacheDic[BindAi].Count >= MaxRecentCommandCacheCount && TimeCacheDic[BindAi].First().AddMinutes(1) > DateTime.Now)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
