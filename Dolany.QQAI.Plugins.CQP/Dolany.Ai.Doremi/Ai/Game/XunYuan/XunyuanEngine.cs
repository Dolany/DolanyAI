using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Xiuxian;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanEngine
    {
        public long GroupNum { get; set; }

        public string BindAi { get; set; }

        public List<XunYuanGamingModel> Gamers { get; set; }

        private XunyuanTreasure TreasureTotal { get; set; } = new XunyuanTreasure();

        private readonly int BasicHPTotal;

        private readonly int BasicAtkTotal;

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
            monster.InitData(BasicHPTotal, BasicAtkTotal);
            PushMsg($"遭遇到了 {monster.Name} !");
            MonsterEncounter(monster);
        }

        private void MonsterEncounter(XunyuanMonsterModel monster)
        {
            while (!monster.IsDead && !IsGameOver())
            {
                PlayersTurn(monster);

                if (monster.IsDead || monster.Name == "逃跑的小偷")
                {
                    break;
                }

                MonsterTurn(monster);
            }

            MonsterSettlement(monster);
        }

        private void PlayersTurn(XunyuanMonsterModel monster)
        {
            var randPlayers = Rander.RandSort(Gamers.ToArray());
            var firstPlayer = randPlayers[0];

            var choice = 0;
            if (randPlayers.Length > 1)
            {
                choice = Waiter.Instance.WaitForOptions(GroupNum, firstPlayer.QQNum, "请做出抉择！", new[] {"攻击怪物", "攻击队友"}, BindAi);
            }

            PlayerAct(firstPlayer, choice, monster, randPlayers.Length > 1 ? randPlayers[1] : null);

            if (Gamers.Count <= 1 || monster.IsDead)
            {
                return;
            }

            var secondPlayer = randPlayers[1];
            choice = Waiter.Instance.WaitForOptions(GroupNum, secondPlayer.QQNum, "请做出抉择！", new[] {"攻击怪物", "攻击队友"}, BindAi);
            PlayerAct(secondPlayer, choice, monster, firstPlayer);
        }

        private void PlayerAct(XunYuanGamingModel player, int choice, XunyuanMonsterModel monster, XunYuanGamingModel partner)
        {
            switch (choice)
            {
                case -1:
                {
                    PushMsg("你放弃了思考！");
                    break;
                }
                case 0:
                {
                    monster.HP = Math.Max(0, monster.HP - player.Attack);
                    PushMsg($"{monster.Name} 受到了 {CodeApi.Code_At(player.QQNum)} 的 {player.Attack} 点伤害，剩余 {monster.HP} 点生命值");
                    break;
                }
                case 1:
                {
                    partner.HP = Math.Max(0, partner.HP - player.Attack);
                    PushMsg($"{CodeApi.Code_At(player.QQNum)} 受到了 {CodeApi.Code_At(player.QQNum)} 的 {player.Attack} 点伤害，剩余 {partner.HP} 点生命值");
                    if (partner.IsDead)
                    {
                        PushMsg($"{CodeApi.Code_At(partner.QQNum)} 被移除了队伍！");
                        Gamers.Remove(partner);
                    }
                    break;
                }
            }
        }

        private void MonsterTurn(XunyuanMonsterModel monster)
        {
            var target = Gamers[Rander.RandInt(Gamers.Count)];

            target.HP = Math.Max(0, target.HP - monster.Atk);
            PushMsg($"{CodeApi.Code_At(target.QQNum)} 受到了 {monster.Name} 的 {monster.Atk}点攻击伤害！剩余 {target.HP}点生命");
            if (!target.IsDead)
            {
                return;
            }

            PushMsg($"{CodeApi.Code_At(target.QQNum)} 被移除了队伍！");
            Gamers.Remove(target);
        }

        private void MonsterSettlement(XunyuanMonsterModel monster)
        {
            if (monster.IsDead)
            {
                if (monster.Name == "逃跑的小偷")
                {
                    var armer = EscapeArmerMgr.Instance.RandArmer();
                    PushMsg($"已经成功击败了 {monster.Name} !获得了 \r{armer.Name} * 1");
                    TreasureTotal.AddEscape(armer.Name);
                }
                else
                {
                    var bonusDic = new Dictionary<string, int>();
                    if (!string.IsNullOrEmpty(monster.DropArmerTag))
                    {
                        var armer = ArmerMgr.Instance.RandTagArmer(monster.DropArmerTag);
                        bonusDic.Add(armer.Name, 1);
                        TreasureTotal.AddArmer(armer.Name);
                    }

                    if (monster.DropGolds > 0)
                    {
                        bonusDic.Add("金币", monster.DropGolds);
                        TreasureTotal.Golds += monster.DropGolds;
                    }

                    PushMsg($"已经成功击败了 {monster.Name} !获得了 \r{string.Join("\r", bonusDic.Select(p => $"{p.Key} * {p.Value}"))}");
                }
            }
            else if(monster.Name == "逃跑的小偷")
            {
                TreasureTotal.Clear();
                PushMsg("很遗憾，所有获得的奖励清空！");
            }
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
