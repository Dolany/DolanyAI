using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Cache
{
    public class RestrictorSvc : IDependency, IDataMgr
    {
        public const int MaxRecentCommandCacheCount = 130;

        public Dictionary<string, int> BindAiLimit = new Dictionary<string, int>();

        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> TimeCacheDic = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();

        public Dictionary<string, int> Pressures =>
            TimeCacheDic.OrderByDescending(p => p.Value.Count).ToDictionary(p => p.Key, p => p.Value.Count);

        public BindAiSvc BindAiSvc { get; set; }
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public void Cache(string BindAi)
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

        public int GetPressure(string BindAi)
        {
            if (!TimeCacheDic.ContainsKey(BindAi) || !TimeCacheDic[BindAi].Any())
            {
                return 0;
            }

            return TimeCacheDic[BindAi].Count;
        }

        public bool IsTooFreq(string BindAi)
        {
            return GetPressure(BindAi) >= BindAiLimit[BindAi];
        }

        public void CleanOutOfDateData()
        {
            foreach (var (_, queue) in TimeCacheDic)
            {
                while (!queue.IsEmpty && queue.TryPeek(out var earlistTime) && earlistTime.AddHours(1) < DateTime.Now)
                {
                    queue.TryDequeue(out _);
                }
            }
        }

        public void RefreshData()
        {
            BindAiLimit = BindAiSvc.AiDic.Keys.ToDictionary(p => p, p => BindAiRestrict.Get(p).MaxLimit);
        }

        public string AllocateBindAi(MsgInformationEx MsgDTO)
        {
            var availableBindAis = new List<BindAiModel>();
            if (MsgDTO.Type == MsgType.Group && GroupSettingSvc[MsgDTO.FromGroup] != null)
            {
                availableBindAis = GroupSettingSvc[MsgDTO.FromGroup].BindAis.Where(p => !IsTooFreq(p))
                    .Select(p => BindAiSvc[p]).ToList();
            }
            else if(!IsTooFreq(MsgDTO.BindAi))
            {
                availableBindAis = new List<BindAiModel>(){BindAiSvc[MsgDTO.BindAi]};
            }

            availableBindAis = availableBindAis.Where(p => p.IsConnected).ToList();
            return availableBindAis.Any() ? availableBindAis.OrderBy(p => GetPressure(p.Name)).First().Name : string.Empty;
        }
    }

    public class BindAiRestrict : DbBaseEntity
    {
        public string BindAi { get; set; }

        public int MaxLimit { get; set; }

        public static BindAiRestrict Get(string BindAi)
        {
            var rec = MongoService<BindAiRestrict>.GetOnly(p => p.BindAi == BindAi);
            if (rec != null)
            {
                return rec;
            }

            rec = new BindAiRestrict(){BindAi = BindAi, MaxLimit = RestrictorSvc.MaxRecentCommandCacheCount};
            MongoService<BindAiRestrict>.Insert(rec);
            return rec;
        }

        public void Update()
        {
            MongoService<BindAiRestrict>.Update(this);
        }
    }
}
