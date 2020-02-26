using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.Game.ChessAgainst
{
    public class ChessAgainstAI : AIBase
    {
        public override string AIName { get; set; } = "对决";

        public override string Description { get; set; } = "AI for Chess Fight.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public override bool NeedManualOpeon { get; } = true;

        private static BindAiMgr BindAiMgr => AutofacSvc.Resolve<BindAiMgr>();
        private static ChessMgr ChessMgr => AutofacSvc.Resolve<ChessMgr>();

        [EnterCommand(ID = "ChessAgainstAI_Fight",
            Command = "对决 决斗",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "指定一名成员进行对决",
            Syntax = "[@QQ号]",
            Tag = "游戏功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool Fight(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];

            if (MsgDTO.FromQQ == aimNum)
            {
                MsgSender.PushMsg(MsgDTO, "你无法跟自己对决！");
                return false;
            }

            if (BindAiMgr.AllAiNums.Contains(aimNum))
            {
                MsgSender.PushMsg(MsgDTO, "鱼唇的人类，你无法挑战ai的威严！");
                return false;
            }

            if (ChessMgr.IsGroupInPlaying(MsgDTO.FromGroup))
            {
                MsgSender.PushMsg(MsgDTO, "本群正在进行一场对决，请稍后再试！");
                return false;
            }

            if (OSPersonBuff.CheckBuff(MsgDTO.FromQQ, "黄砂"))
            {
                MsgSender.PushMsg(MsgDTO, "你当前无法进行挑战！(黄砂)");
                return false;
            }

            if (!Waiter.WaitForConfirm(MsgDTO.FromGroup, aimNum, $"{CodeApi.Code_At(MsgDTO.FromQQ)} 正在向你发起一场对决，是否接受？",MsgDTO.BindAi, 10))
            {
                MsgSender.PushMsg(MsgDTO, "对决取消！");
                return false;
            }

            if (ChessMgr.IsQQInPlaying(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你正在进行一场对决，请稍后再试！");
                return false;
            }

            if (ChessMgr.IsQQInPlaying(aimNum))
            {
                MsgSender.PushMsg(MsgDTO, "你的对手正在进行一场对决，请稍后再试！");
                return false;
            }

            ChessMgr.StartAGame(MsgDTO.FromGroup, MsgDTO.FromQQ, aimNum, (GroupNum, QQNum, Msg, judge) =>
            {
                var msg = MsgDTO.Clone();
                msg.FromQQ = QQNum;
                msg.FromGroup = GroupNum;

                var info = Waiter.WaitForInformation(msg, Msg,
                    information => information.FromGroup == GroupNum && information.FromQQ == QQNum && judge(information.Msg), 10, true);
                return info == null ? string.Empty : info.Msg;
            }, MsgDTO.BindAi);

            return true;
        }
    }
}
