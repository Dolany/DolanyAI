using System;
using System.Linq;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.Expedition
{
    public class ExpeditionRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public DateTime EndTime { get; set; }

        public string Scene { get; set; }

        /// <summary>
        /// 是否已领取奖励
        /// </summary>
        public bool IsDrawn { get; set; }

        public bool IsExpediting => DateTime.Now < EndTime.ToLocalTime();

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
