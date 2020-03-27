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
            TimeCacheDic.OrderByDescending(p => p.Value.Count).ToDictionary(p => p.Key, p => p.Value.Count);

        public static void Cache(string BindAi)
        {
            if (TimeCacheDic.ContainsKey(BindAi))
            {
                var tc = TimeCacheDic[BindAi];
                tc.Enqueue(DateTime.Now);
            }
            else
            {
                var queue = new ConcurrentQueue<DateTime>();
                queue.Enqueue(DateTime.Now);
                TimeCacheDic.TryAdd(BindAi, queue);
            }
        }

        public static int GetPressure(string BindAi)
        {
            if (!TimeCacheDic.ContainsKey(BindAi) || !TimeCacheDic[BindAi].Any())
            {
                return 0;
            }

            return TimeCacheDic[BindAi].Count;
        }

        public static bool IsTooFreq(string BindAi)
        {
            return GetPressure(BindAi) >= MaxRecentCommandCacheCount;
        }

        public static void Refresh()
        {
            foreach (var (_, queue) in TimeCacheDic)
            {
                while (!queue.IsEmpty)
                {
                    if (queue.TryPeek(out var earlistTime) && earlistTime.AddHours(1) < DateTime.Now)
                    {
                        queue.TryDequeue(out _);
                        continue;
                    }

                    break;
                }
            }
        }
    }
}
