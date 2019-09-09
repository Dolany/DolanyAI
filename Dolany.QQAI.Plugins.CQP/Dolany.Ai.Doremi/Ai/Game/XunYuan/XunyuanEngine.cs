using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.Cache;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanEngine
    {
        public long GroupNum { get; set; }

        public string BindAi { get; set; }

        public List<XunYuanGamingModel> Gamers { get; set; }

        private XunyuanTreasure TreasureTotal { get; set; } = new XunyuanTreasure();

        private int BasicHPTotal;

        private int BasicAtkTotal;

        private XunyuanCaveModel Cave;

        private const int MaxTurn = 3;

        public XunyuanEngine(XunYuanGamingModel[] Gamers, long GroupNum, string BindAi)
        {
            this.Gamers = Rander.RandSort(Gamers).ToList();
            this.GroupNum = GroupNum;
            this.BindAi = BindAi;

            BasicHPTotal = Gamers.Sum(g => g.BasicHP);
            BasicAtkTotal = Gamers.Sum(g => g.BasicAttack);
        }

        public void StartGame()
        {
            Cave = XunyuanCaveMgr.Instance.RandCaves;
            PushMsg($"寻缘开始！当前副本为：{Cave.Name}");

            for (var i = 0; i < MaxTurn; i++)
            {
                ProceedTurn();

                if (IsGameOver())
                {
                    break;
                }
            }

            JudgeResult();
            PushMsg("寻缘结束！");
        }

        private void ProceedTurn()
        {
            var monster = Cave.RandMonster;
            PushMsg($"遭遇到了 {monster.Name} !");
            MonsterEncounter(monster);
        }

        private void MonsterEncounter(XunyuanMonsterModel monster)
        {
            // todo
        }

        private void PushMsg(string msg)
        {
            Thread.Sleep(1500);
            MsgSender.PushMsg(GroupNum, 0, msg, BindAi);
        }

        private bool IsGameOver()
        {
            return Gamers.IsNullOrEmpty();
        }

        private void JudgeResult()
        {
            if (IsGameOver())
            {
                PushMsg("很遗憾队伍全灭，请再接再厉！");
                return;
            }

            if (Gamers.Count == 1)
            {
                var winner = Gamers.First();
                var treasureStr = TreasureTotal.ToString();
                if (string.IsNullOrEmpty(treasureStr))
                {
                    PushMsg($"获胜者为 {CodeApi.Code_At(winner.QQNum)} !但是没有任何物品掉落");
                    return;
                }

                PushMsg($"获胜者为 {CodeApi.Code_At(winner.QQNum)} !获得以下物品：\r{treasureStr}");
                TreasureTotal.SaveToPerson(winner.QQNum);
                return;
            }

            var msg = "恭喜你们获得了胜利！\r";
            var treasures = TreasureTotal.Split();
            msg += $"{CodeApi.Code_At(Gamers[0].QQNum)} 获取了\r{treasures[0]}\r{CodeApi.Code_At(Gamers[1].QQNum)} 获取了\r{treasures[1]}";
            PushMsg(msg);

            treasures[0].SaveToPerson(Gamers[0].QQNum);
            treasures[1].SaveToPerson(Gamers[1].QQNum);
        }
    }
}
