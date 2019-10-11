using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Explore
{
    public class ExplorePlayer : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string CurArea { get; set; }

        public Dictionary<string, int> ToolDic { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> MaterialDic { get; set; } = new Dictionary<string, int>();

        public List<ExploreCarrerData> CarrerDatas { get; set; } = new List<ExploreCarrerData>();

        public static ExplorePlayer Get(long QQNum)
        {
            var player = MongoService<ExplorePlayer>.GetOnly(p => p.QQNum == QQNum);
            if (player != null)
            {
                return player;
            }

            player = new ExplorePlayer(){QQNum = QQNum};
            MongoService<ExplorePlayer>.Insert(player);

            return player;
        }

        public void CarrerExpGain(string carrerName, int exp)
        {
            var carrer = CarrerDatas.First(p => p.Name == carrerName);
            carrer.Exp += exp;
        }

        public void Update()
        {
            ToolDic.Remove(p => p == 0);
            MaterialDic.Remove(p => p == 0);

            MongoService<ExplorePlayer>.Update(this);
        }
    }

    public class ExploreCarrerData
    {
        public string Name { get; set; }

        public int Exp { get; set; }

        public int Level { get; set; }
    }
}
