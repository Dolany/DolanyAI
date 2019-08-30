using Dolany.Ai.Core.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.OnlineStore
{
    [BsonIgnoreExtraElements]
    public partial class OSPerson
    {
        public int Level { get; set; }
        public int MaxHP { get; set; }

        public string EmojiLevel => Utility.LevelEmoji(Level);
    }
}
