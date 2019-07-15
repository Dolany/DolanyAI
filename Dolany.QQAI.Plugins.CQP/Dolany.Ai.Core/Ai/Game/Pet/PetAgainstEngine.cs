using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetAgainstEngine
    {
        public long GroupNum { get; set; }

        public string BindAi { private get; set; }

        public GamingPet SelfPet { get; set; }

        public GamingPet AimPet { get; set; }

        private GamingPet Winner { get; set; }

        public void StartGame()
        {
            BeforeStart();

            for (var i = 0; i < 10 && !JudgeWinner(); i++)
            {
                // todo

                Switch();
            }

            JudgeResult();
        }

        private void BeforeStart()
        {
            var msg = $"{SelfPet.Name}(lv.{SelfPet.Level})   VS   {AimPet.Name}(lv.{AimPet.Level})" +
                      "\r对决即将开始，请双方做好准备！";
            SendMessage(msg);

            JudgeFirst();
            var skillName = LevelUpBonus();
            msg = $"${SelfPet.Name}获得了先手\r{AimPet.Name}的{skillName}获得等级+1";
            SendMessage(msg);
        }

        private void JudgeFirst()
        {
            if (CommonUtil.RandInt(1) == 0)
            {
                return;
            }

            Switch();
        }

        private void Switch()
        {
            var temp = SelfPet;
            SelfPet = AimPet;
            AimPet = temp;
        }

        private string LevelUpBonus()
        {
            var (key, _) = AimPet.Skills.Where(p => p.Value < 5).RandElement();
            AimPet.Skills[key]++;
            return key;
        }

        private void SendMessage(string msg)
        {
            MsgSender.PushMsg(GroupNum, 0, msg, BindAi);
        }

        private bool JudgeWinner()
        {
            if (SelfPet.HP > 0 && AimPet.HP > 0)
            {
                return false;
            }

            if (SelfPet.HP <= 0 && AimPet.HP <= 0)
            {
                return true;
            }

            Winner = SelfPet.HP > 0 ? SelfPet : AimPet;
            return true;
        }

        private void JudgeResult()
        {

        }
    }
}
