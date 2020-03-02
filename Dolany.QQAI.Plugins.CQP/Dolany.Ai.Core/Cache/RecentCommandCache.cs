using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Cache
{
    public class RecentCommandCache
    {
        public static readonly int MaxRecentCommandCacheCount = Global.DefaultConfig.MaxRecentCommandCacheCount;

        private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> TimeCacheDic = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();

        public static Dictionary<string, int> Pressures =>
            TimeCacheDic.ToDictionary(p => p.Key, p => p.Value.Count(d => d.AddHours(1) > DateTime.Now)).OrderByDescending(p => p.Value).ToDictionary(p => p.Key, p => p.Value);

        public static void Cache(string BindAi)
        {
            if (TimeCacheDic.ContainsKey(BindAi))
            {
                var tc = TimeCacheDic[BindAi];
                tc.Enqueue(DateTime.Now);
                if (tc.Count > MaxRecentCommandCacheCount)
                {
                    tc.TryDequeue(out _);
                }
            }
            else
            {
                var queue = new ConcurrentQueue<DateTime>();
                queue.Enqueue(DateTime.Now);
                TimeCacheDic.TryAdd(BindAi, queue);
            }
        }

        public static bool IsTooFreq(string BindAi)
        {
            if (!TimeCacheDic.ContainsKey(BindAi) || !TimeCacheDic[BindAi].Any())
            {
                return false;
            }

            return TimeCacheDic[BindAi].Count >= MaxRecentCommandCacheCount && TimeCacheDic[BindAi].TryPeek(out var peekTime) && peekTime.AddHours(1) > DateTime.Now;
        }
    }
}
