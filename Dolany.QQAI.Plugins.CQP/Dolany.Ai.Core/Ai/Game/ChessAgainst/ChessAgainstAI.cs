﻿using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Game.Chess;

namespace Dolany.Ai.Core.Ai.Game.ChessAgainst
{
    [AI(
        Name = "对决",
        Description = "AI for Chess Fight.",
        Enable = false,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class ChessAgainstAI : AIBase
    {
        [EnterCommand(Command = "对决",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "指定一名成员进行对决",
            Syntax = "[@QQ号]",
            Tag = "游戏功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public void Sell(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];

            if (ChessMgr.Instance.IsGroupInPlaying(MsgDTO.FromGroup))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "本群正在进行一场对决，请稍后再试！");
                return;
            }

            if (ChessMgr.Instance.IsQQInPlaying(MsgDTO.FromQQ))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你正在进行一场对决，请稍后再试！");
                return;
            }

            if (ChessMgr.Instance.IsQQInPlaying(aimNum))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的对手正在进行一场对决，请稍后再试！");
                return;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO.FromGroup, aimNum, $"{CodeApi.Code_At(MsgDTO.FromQQ)} 正在向你发起一场对决，是否接受？", 10))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "对决取消！");
                return;
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
        }
    }
}