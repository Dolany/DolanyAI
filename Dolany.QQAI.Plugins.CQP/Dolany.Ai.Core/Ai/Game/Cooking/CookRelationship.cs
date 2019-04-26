using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Cooking
{
    public class CookRelationship : DbBaseEntity
    {
        public long[] QQPair { get; set; }

        public int Relationship { get; set; }
    }
}
