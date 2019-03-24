using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Game.Advanture.Cave;
using Dolany.Game.OnlineStore;

namespace Dolany.Game.Advanture
{
    public class AdvGameEngine
    {
        public readonly long GroupNum;

        public readonly AdvPlayer[] players;

        private readonly CaveDataModel CaveModel;

        private int GamingIdx;

        private readonly List<ICave> CaveList = new List<ICave>();

        private AdvPlayer CurPlayer => players[GamingIdx];
        private AdvPlayer OtherPlayer => players[(GamingIdx + 1) % players.Length];

        private AdvPlayer Winner;

        private int Bonus;

        public AdvGameEngine(AdvPlayer[] players, long GroupNum, int CaveNo)
        {
            this.players = players;
            this.GroupNum = GroupNum;
            this.CaveModel = CaveSettingHelper.Instance.GetCaveByNo(CaveNo);

            for (var i = 0; i < 3; i++)
            {
                CaveList.Add(CaveModel.NextCave());
            }
        }

        public void GameStart()
        {
            CommonUtil.MsgSendBack(GroupNum, 0, $"冒险开始！当前副本是 {CaveModel.Name} ！");
            Thread.Sleep(1000);

            try
            {
                while (Winner == null)
                {
                    ProcessTurn();
                    AlterTurn();
                    Thread.Sleep(1000);
                }

                Settlement();
            }
            catch(Exception ex)
            {
                CommonUtil.MsgSendBack(GroupNum, 0, "游戏异常，对决结束！");
                RuntimeLogger.Log(ex);
            }
        }

        private void ProcessTurn()
        {
            var msg = $"回合开始！请选择合适的数字！\r{PrintCaves()}";
            var response = CommonUtil.WaitForNumFunc(GroupNum, CurPlayer.QQNum, msg, i => i > 0 && i <= CaveList.Count);
            if (response < 0)
            {
                response = 1;
            }

            ProcessCave(CaveList[response - 1]);
            RefreshCave(response - 1);

            CheckWinner();
        }

        private void ProcessCave(ICave cave)
        {
            var msg = $"你遇到了 {cave.Description} ！";
            CommonUtil.MsgSendBack(GroupNum, CurPlayer.QQNum, msg);
            Thread.Sleep(1000);

            switch (cave.Type)
            {
                case CaveType.宝箱:
                    ProcessTreasureCave(cave as TreasureCave);
                    break;
                case CaveType.陷阱:
                    ProcessTrapCave(cave as TrapCave);
                    break;
                case CaveType.怪兽:
                    ProcessMonsterCave(cave as MonsterCave);
                    break;
            }

            cave.Visible = true;
        }

        private void ProcessMonsterCave(MonsterCave cave)
        {
            var atk = CurPlayer.GetAtk();
            cave.HP = Math.Max(cave.HP - atk, 0);
            CommonUtil.MsgSendBack(GroupNum, CurPlayer.QQNum, 
                $"你打出了 {atk} 点伤害，{cave.Name}剩余{cave.HP}{Emoji.心}");
            Thread.Sleep(1000);
            if (cave.HP > 0)
            {
                CurPlayer.HP = Math.Max(CurPlayer.HP - cave.Atk, 0);
                CommonUtil.MsgSendBack(GroupNum, CurPlayer.QQNum, 
                    $"{cave.Name}对你造成了{cave.Atk}点伤害，你剩余{CurPlayer.HP}{Emoji.心}");
            }
            else
            {
                OtherPlayer.HP = Math.Max(OtherPlayer.HP - cave.Atk, 0);
                CommonUtil.MsgSendBack(GroupNum, OtherPlayer.QQNum, 
                    $"你受到了{cave.Atk}点伤害，你剩余{OtherPlayer.HP}{Emoji.心}");
            }
        }

        private void ProcessTreasureCave(TreasureCave cave)
        {
            var atk = CurPlayer.GetAtk();
            cave.HP = Math.Max(cave.HP - atk, 0);
            CommonUtil.MsgSendBack(GroupNum, CurPlayer.QQNum, 
                $"你打出了 {atk} 点伤害，{cave.Name}剩余{cave.HP}{Emoji.心}");
            Thread.Sleep(1000);
            if (cave.HP > 0)
            {
                return;
            }

            Bonus += cave.Golds;
            CommonUtil.MsgSendBack(GroupNum, CurPlayer.QQNum,
                $"你击碎了宝箱！当前赏金为 {Bonus}{Emoji.钱}！赢得对决的人将获得全部赏金！");
        }

        private void ProcessTrapCave(TrapCave cave)
        {
            CurPlayer.HP = Math.Max(CurPlayer.HP -= cave.Atk, 0);
            CommonUtil.MsgSendBack(GroupNum, CurPlayer.QQNum, 
                $"你受到了 {cave.Atk}点伤害，剩余生命值 {CurPlayer.HP}{Emoji.心}");
        }

        private void RefreshCave(int idx)
        {
            if (CaveList[idx].IsNeedRefresh)
            {
                CaveList[idx] = CaveModel.NextCave();
            }
        }

        private void CheckWinner()
        {
            for (var i = 0; i < players.Length; i++)
            {
                if (players[i].HP > 0)
                {
                    continue;
                }

                Winner = players[(i + 1) % players.Length];
                return;
            }
        }

        private void AlterTurn()
        {
            GamingIdx = (GamingIdx + 1) % players.Length;
        }

        private void Settlement()
        {
            var msg = $"对决结束！{CodeApi.Code_At(Winner.QQNum)}获取了胜利！";
            
            if (Bonus > 0)
            {
                msg += $"\r获得了全部赏金 {Bonus}{Emoji.钱}！";
                var osPerson = OSPerson.GetPerson(Winner.QQNum);
                osPerson.Golds += Bonus;
                osPerson.Update();
            }
            CommonUtil.MsgSendBack(GroupNum, 0, msg);

            foreach (var player in players)
            {
                var p = AdvPlayer.GetPlayer(player.QQNum);
                p.BattleRecord(p.QQNum == Winner.QQNum);
                p.Update();
                if (p.QQNum != Winner.QQNum || p.WinTotal % 5 != 0)
                {
                    continue;
                }

                var items = HonorHelper.Instance.CurMonthLimitItems();
                var item = items[CommonUtil.RandInt(items.Length)];
                CommonUtil.MsgSendBack(GroupNum, p.QQNum, 
                    $"你已经累计赢得 {p.WinTotal}场对决，获取额外奖励 {item.Name}*1");

                var (honorMsg, _) = ItemHelper.Instance.ItemIncome(p.QQNum, item.Name);
                if (!string.IsNullOrEmpty(honorMsg))
                {
                    CommonUtil.MsgSendBack(GroupNum, 0, honorMsg);
                }
            }
        }

        private string PrintCaves()
        {
            var msgList = CaveList.Select((t, i) => $"{i + 1}:{(t.Visible ? t.Description : "未知")}").ToList();

            return string.Join("\r", msgList);
        }
    }
}
