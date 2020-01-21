using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.SwordExplore
{
    public class SERecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int MaxHP { get; set; }

        public int CurHP { get; set; }

        public int MaxEndurance { get; set; }

        public int CurEndurance { get; set; }

        public string CurScene { get; set; }

        public List<string> ClearAreas { get; set; } = new List<string>();

        public Dictionary<string, int> Materials { get; set; } = new Dictionary<string, int>();

        public SuperWeapon SuperWeapon { get; set; }

        public static SERecord Get(long QQNum)
        {
            var rec = MongoService<SERecord>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
                return rec;
            }

            rec = new SERecord(){QQNum = QQNum, CurScene = SEMapMgr.Instance.DefaultScene.ID};
            MongoService<SERecord>.Insert(rec);
            return rec;
        }

        public void Update()
        {
            MongoService<SERecord>.Update(this);
        }
    }

    public class SuperWeapon
    {
        public string Name { get; set; }

        public string Kind { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public List<MagicJewelry> EmbedJewelrys { get; set; } = new List<MagicJewelry>();
    }

    public class MagicJewelry
    {
        public string Name { get; set; }

        public string Kind { get; set; }

        public Dictionary<string, int> Affects { get; set; } = new Dictionary<string, int>();
    }
}
