using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Cache
{
    public class RestrictorSvc : IDependency
    {
        public const int MaxRecentCommandCacheCount = 130;

        public static Dictionary<string, int> BindAiLimit =>
            RapidCacher.GetCache("BindAiLimitDic", TimeSpan.FromMinutes(5), () => BindAiSvc.AiDic.Keys.ToDictionary(p => p, p => BindAiRestrict.Get(p).MaxLimit));

        public static Dictionary<string, int> Pressures =>
            BindAiSvc.AiDic.Keys.Select(p => new {Name = p, Pressure = GetPressure(p)}).OrderByDescending(p => p.Pressure)
                .ToDictionary(p => p.Name, p => p.Pressure);

        public BindAiSvc BindAiSvc { get; set; }
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public static void Cache(string BindAi)
        {
            RestrictRec.Cache(BindAi);
        }

        public static int GetPressure(string BindAi)
        {
            return RestrictRec.Pressure(BindAi);
        }

        public static bool IsTooFreq(string BindAi)
        {
            return GetPressure(BindAi) >= BindAiLimit[BindAi];
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

    public class RestrictRec : DbBaseEntity
    {
        public string BindAi { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CmdTime { get; set; }

        public static void Cache(string BindAi)
        {
            MongoService<RestrictRec>.Insert(new RestrictRec(){BindAi = BindAi, CmdTime = DateTime.Now});
        }

        public static int Pressure(string BindAi)
        {
            return (int) MongoService<RestrictRec>.Count(p => p.BindAi == BindAi);
        }
    }
}
