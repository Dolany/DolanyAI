﻿using Dolany.Ai.Common;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Game.Chess;
using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.ChessAgainst
{
    [AI(
        Name = "对决",
        Description = "AI for Chess Fight.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class ChessAgainstAI : AIBase
    {
        [EnterCommand(Command = "对决 决斗",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "指定一名成员进行对决",
            Syntax = "[@QQ号]",
            Tag = "游戏功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public bool Sell(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];

            if (MsgDTO.FromQQ == aimNum)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你无法跟自己对决！");
                return false;
            }

            if (ChessMgr.Instance.IsGroupInPlaying(MsgDTO.FromGroup))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "本群正在进行一场对决，请稍后再试！");
                return false;
            }

            if (ChessMgr.Instance.IsQQInPlaying(MsgDTO.FromQQ))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你正在进行一场对决，请稍后再试！");
                return false;
            }

            if (ChessMgr.Instance.IsQQInPlaying(aimNum))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的对手正在进行一场对决，请稍后再试！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.CheckBuff("黄砂"))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你当前无法进行挑战！(黄砂)");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO.FromGroup, aimNum, $"{CodeApi.Code_At(MsgDTO.FromQQ)} 正在向你发起一场对决，是否接受？", 10))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "对决取消！");
                return false;
            }

            ChessMgr.Instance.StartAGame(MsgDTO.FromGroup, MsgDTO.FromQQ, aimNum, (Msg, GroupNum, QQNum) =>
            {
                var msg = MsgDTO.Clone();
                msg.FromQQ = QQNum;
                msg.FromGroup = GroupNum;

                MsgSender.Instance.PushMsg(msg, Msg, QQNum != 0);
            }, (GroupNum, QQNum, Msg, judge) =>
            {
                var msg = MsgDTO.Clone();
                msg.FromQQ = QQNum;
                msg.FromGroup = GroupNum;

                var info = Waiter.Instance.WaitForInformation(msg, Msg,
                    information => information.FromGroup == GroupNum && information.FromQQ == QQNum && judge(information.Msg), 10, true);
                return info == null ? string.Empty : info.Msg;
            });

            return true;
        }

        [EnterCommand(
            Command = "驱散",
            Description = "清除某人身上的所有buff",
            Syntax = "[@QQ]",
            Tag = "对决",
            SyntaxChecker = "At",
            AuthorityLevel = AuthorityLevel.群主,
            IsPrivateAvailable = false)]
        public bool Dispel(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];

            var sourcePerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (sourcePerson.Golds < 500)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "驱散全部buff需要500金币，你没有足够的金币！");
                return false;
            }

            var aimPerson = OSPerson.GetPerson(qqNum);
            aimPerson.Buffs.Clear();
            aimPerson.Update();

            sourcePerson.Golds -= 500;
            sourcePerson.Update();

            MsgSender.Instance.PushMsg(MsgDTO, "驱散成功！");
            return true;
        }

        [EnterCommand(
            Command = "驱散",
            Description = "清除某人身上的指定buff",
            Syntax = "[@QQ] [Buff名称]",
            Tag = "对决",
            SyntaxChecker = "At Word",
            AuthorityLevel = AuthorityLevel.群主,
            IsPrivateAvailable = false)]
        public bool DispelOneBuff(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long) param[0];
            var buffName = param[1] as string;

            var aimPerson = OSPerson.GetPerson(qqNum);
            if (!aimPerson.CheckBuff(buffName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "目标身上没有指定buff！");
                return false;
            }

            var sourcePerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (sourcePerson.Golds < 100)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "驱散该buff需要100金币，你没有足够的金币！");
                return false;
            }

            aimPerson.RemoveBuff(buffName);
            aimPerson.Update();

            sourcePerson.Golds -= 100;
            sourcePerson.Update();

            MsgSender.Instance.PushMsg(MsgDTO, "驱散成功！");
            return true;
        }
    }
}
