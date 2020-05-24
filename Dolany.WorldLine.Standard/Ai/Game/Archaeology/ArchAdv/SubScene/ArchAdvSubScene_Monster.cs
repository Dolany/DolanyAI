using Dolany.Ai.Common.Models;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology.ArchAdv
{
    public class ArchAdvSubScene_Monster : IArchAdvSubScene
    {
        public string ArchType { get; set; } = "Monster";
        public ArchaeologySubSceneModel Scene { get; set; }
        public int Level { get; set; }
        public MsgInformationEx MsgDTO { get; set; }

        private ArchMonster Monster { get; set; }

        public bool StartAdv()
        {
            Monster = Scene.Data.DictionaryToObject<ArchMonster>();
            Monster.LevelAdjust(Level);

            // todo
            return true;
        }
    }

    public class ArchMonster
    {
        public string Name { get; set; }

        public int SAN { get; set; }

        public string MainAttr { get; set; }

        public int AttrValue { get; set; }

        public ArchMonsterAttackMode AttackMode { get; set; }

        public void LevelAdjust(int level)
        {
            SAN += SAN / 2 * level;
            AttrValue += AttrValue / 2 * level;
        }
    }

    public enum ArchMonsterAttackMode
    {
        Balence = 0, // 平衡攻击，攻击相同的属性
        Weak = 1, // 弱攻击，攻击克制自身的属性，且伤害减半
        Strong = 2 // 强攻击，攻击被自身克制的属性，且伤害加倍
    }
}
