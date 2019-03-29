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

        public void Update()
        {
            MongoService<GroupSettings>.Update(this);
        }

        public bool HasFunction(string name)
        {
            return EnabledFunctions.Contains(name);
        }
    }
}
