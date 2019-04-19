using System.Collections.Generic;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Ai.Game.Alchemy
{
    [BsonIgnoreExtraElements]
    public class AlchemyPlayer : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string Name { get; set; }

        public int Level { get; set; } = 1;

        public List<string> Titles { get; set; } = new List<string>();
    }
}
