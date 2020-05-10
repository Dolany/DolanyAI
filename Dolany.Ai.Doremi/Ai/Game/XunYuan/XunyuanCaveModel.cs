using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Doremi.Ai.Game.XunYuan
{
    public class XunyuanCaveModel : INamedJsonModel
    {
        public string Name { get; set; }

        public List<XunyuanMonsterModel> Monsters { get; set; }

        public XunyuanMonsterModel RandMonster => Monsters.RandElement();
    }

    public class XunyuanMonsterModel
    {
        public string Name { get; set; }

        public int HPTimes { get; set; }

        public int AttackTimes { get; set; }

        public string DropArmerTag { get; set; }

        public int DropGolds { get; set; }

        public int HP { get; set; }

        public int Atk { get; set; }

        public bool IsDead => HP == 0;

        public void InitData(int BasicHP, int BasicAtk)
        {
            HP = BasicHP * HPTimes;
            Atk = BasicAtk * AttackTimes;
        }
    }
}
