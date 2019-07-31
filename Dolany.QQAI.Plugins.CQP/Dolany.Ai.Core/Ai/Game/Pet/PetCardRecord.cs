using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetCardRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int HP { get; set; } = 50;

        public int MaxHP { get; set; } = 100;

        public List<PetCardModel> Cards { get; set; } = new List<PetCardModel>();

        public static PetCardRecord Get(long QQNum)
        {
            var record = MongoService<PetCardRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new PetCardRecord(){QQNum = QQNum};
            MongoService<PetCardRecord>.Insert(record);
            return record;
        }

        public void Update()
        {
            Cards.RemoveAll(p => p.Count == 0);
            MongoService<PetCardRecord>.Update(this);
        }
    }

    public class PetCardModel
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public string Catalog { get; set; }
    }
}
