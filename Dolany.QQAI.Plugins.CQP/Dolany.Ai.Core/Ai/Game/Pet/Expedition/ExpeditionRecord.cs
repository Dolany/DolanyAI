using System;
using System.Linq;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Pet.Expedition
{
    public class ExpeditionRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public DateTime EndTime { get; set; }

        public string Scene { get; set; }

        public bool IsDrawn { get; set; }

        public static ExpeditionRecord GetLastest(long QQNum)
        {
            return MongoService<ExpeditionRecord>.Get(p => p.QQNum == QQNum, p => p.EndTime, false, 0, 1).FirstOrDefault();
        }

        public void Insert()
        {
            MongoService<ExpeditionRecord>.Insert(this);
        }

        public void Update()
        {
            MongoService<ExpeditionRecord>.Update(this);
        }
    }
}
