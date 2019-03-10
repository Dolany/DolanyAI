using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;
using Dolany.Game.OnlineStore;

namespace Dolany.Game.Alchemy
{
    public class GameGlobleMgr
    {
        public static void DoDemage(UPModel source, UPModel aim,int value, long GroupNum)
        {
            var msg = $"对目标造成了 {value} 点伤害\r";
            aim.AlPlayer.CurHP = Math.Max(aim.AlPlayer.CurHP - value, 0);
            if (aim.AlPlayer.CurHP > 0)
            {
                msg += $"目标剩余血量 {aim.AlPlayer.CurHP} 点";
                CommonUtil.MsgSendBack(GroupNum, source.AlPlayer.QQNum, msg, false);
                return;
            }

            msg += "已击杀目标！\r";
            SCacheService.Cache($"AlPlayerAliveCache-{aim.AlPlayer.QQNum}", new AlPlayerAliveCache
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(4),
                QQNum = aim.AlPlayer.QQNum
            }, DateTime.Now.AddHours(4));

            if (aim.AlPlayer.AlItems.Any() && CommonUtil.RandInt(100) < 50)
            {
                var idx = CommonUtil.RandInt(aim.AlPlayer.AlItems.Count);
                var item = aim.AlPlayer.AlItems.Keys.ElementAt(idx);
                aim.AlPlayer.ItemConsume(item);
                source.AlPlayer.ItemGain(item);

                msg += $"抢夺目标{item}*1";
                CommonUtil.MsgSendBack(GroupNum, source.AlPlayer.QQNum, msg, false);
                return;
            }

            if (aim.AlPlayer.MagicDirt.Any() && CommonUtil.RandInt(100) < 50)
            {
                var idx = CommonUtil.RandInt(aim.AlPlayer.MagicDirt.Count);
                var magicDirt = aim.AlPlayer.MagicDirt.Keys.ElementAt(idx);
                aim.AlPlayer.MagicDirtConsume(magicDirt);
                source.AlPlayer.MagicDirtGain(magicDirt);

                msg += $"抢夺目标{magicDirt}*1";
                CommonUtil.MsgSendBack(GroupNum, source.AlPlayer.QQNum, msg, false);
                return;
            }

            if (aim.DriftItemRecord.ItemCount.Any() && CommonUtil.RandInt(100) < 50)
            {
                var idx = CommonUtil.RandInt(aim.DriftItemRecord.ItemCount.Count);
                var item = aim.DriftItemRecord.ItemCount[idx];
                aim.DriftItemRecord.ItemConsume(item.Name);
                source.DriftItemRecord.ItemGain(item.Name);

                msg += $"抢夺目标{item.Name}*1";
                CommonUtil.MsgSendBack(GroupNum, source.AlPlayer.QQNum, msg, false);
                return;
            }

            aim.OSPerson.Golds -= 200;
            source.OSPerson.Golds += 200;

            msg += "抢夺目标200金币";
            CommonUtil.MsgSendBack(GroupNum, source.AlPlayer.QQNum, msg, false);
        }
    }

    public class UPModel
    {
        public readonly AlPlayer AlPlayer;
        public readonly DriftItemRecord DriftItemRecord;
        public readonly OSPerson OSPerson;

        public UPModel(long QQNum)
        {
            AlPlayer = AlPlayer.GetPlayer(QQNum);
            DriftItemRecord = DriftItemRecord.GetRecord(QQNum);
            OSPerson = OSPerson.GetPerson(QQNum);
        }

        public void Update()
        {
            AlPlayer.Update();
            DriftItemRecord.Update();
            OSPerson.Update();
        }
    }
}
