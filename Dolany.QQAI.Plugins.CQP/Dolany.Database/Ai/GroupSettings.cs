using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class GroupSettings : BaseEntity
    {
        public long GroupNum { get; set; }

        public string Name { get; set; }

        public bool IsPowerOn { get; set; } = true;

        public bool ForcedShutDown { get; set; } = false;

        public IList<string> EnabledFunctions { get; set; } = new List<string>();

        public string BindAi { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public GroupAuthInfoModel AuthInfo { get; set; }

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
