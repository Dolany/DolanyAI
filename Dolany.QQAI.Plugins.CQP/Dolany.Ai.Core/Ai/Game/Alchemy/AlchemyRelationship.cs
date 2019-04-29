using System.Linq;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Alchemy
{
    public class AlchemyRelationship : DbBaseEntity
    {
        public long[] QQPair { get; set; }

        public int Relationship { get; set; }

        public static AlchemyRelationship GetRelationship(long firstQQ, long secondQQ)
        {
            var record = MongoService<AlchemyRelationship>.GetOnly(p => p.QQPair.Contains(firstQQ) && p.QQPair.Contains(secondQQ));
            if (record != null)
            {
                return record;
            }

            record = new AlchemyRelationship() {QQPair = new[] {firstQQ, secondQQ}};
            MongoService<AlchemyRelationship>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<AlchemyRelationship>.Update(this);
        }
    }
}
