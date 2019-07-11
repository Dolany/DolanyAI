using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public static class PetExtent
    {
        public static string[] AllAttributes = {"钢铁", "海洋", "深渊", "自然", "神秘"};

        public static void ExtGain(this PetRecord pet, MsgInformationEx MsgDTO, int exp)
        {
            var remainExp = pet.Exp + exp;
            var lvl = pet.Level;
            var levelMode = PetLevelMgr.Instance[lvl];
            while (levelMode.Exp < remainExp)
            {
                remainExp -= levelMode.Exp;
                lvl++;
                levelMode = PetLevelMgr.Instance[lvl];
            }

            var msg = $"{pet.Name}获得了 {exp} 点经验值！";
            if (lvl > pet.Level)
            {
                var points = (lvl - pet.Level) * 2;
                msg += $"\r恭喜{pet.Name}升到了 {lvl} 级！\r" +
                       $"{pet.Name}获得了 {points} 点技能点！";

                pet.RemainSkillPoints += points;
            }

            pet.Level = lvl;
            pet.Exp = remainExp;
            pet.Update();

            MsgSender.PushMsg(MsgDTO, msg);
        }
    }
}
