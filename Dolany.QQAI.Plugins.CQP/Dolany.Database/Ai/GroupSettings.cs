using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class GroupSettings : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public string Name { get; set; }

        public bool IsPowerOn { get; set; } = true;

        public bool ForcedShutDown { get; set; }

        public IList<string> EnabledFunctions { get; set; } = new List<string>();

        public string BindAi { get; set; }

        public List<string> BindAis { get; set; } = new List<string>();

        public DateTime? ExpiryTime { get; set; }

        public GroupAuthInfoModel AuthInfo { get; set; }

        public Dictionary<string, string> AdditionSettings { get; set; } = new Dictionary<string, string>();

        public string WorldLine { get; set; }

        public static GroupSettings Get(long GroupNum)
        {
            return MongoService<GroupSettings>.GetOnly(p => p.GroupNum == GroupNum);
        }

        public void Update()
        {
            MongoService<GroupSettings>.Update(this);
        }

        public bool HasFunction(string name)
        {
            return EnabledFunctions.Contains(name);
        }
    }

    [BsonIgnoreExtraElements]
    public class GroupAuthInfoModel
    {
        public long Owner { get; set; }

        public List<long> Mgrs { get; set; } = new List<long>();
    }
}
