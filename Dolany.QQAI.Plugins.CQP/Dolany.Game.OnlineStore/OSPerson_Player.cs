using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Game.OnlineStore
{
    [BsonIgnoreExtraElements]
    public partial class OSPerson
    {
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int MaxMP { get; set; }
    }
}
