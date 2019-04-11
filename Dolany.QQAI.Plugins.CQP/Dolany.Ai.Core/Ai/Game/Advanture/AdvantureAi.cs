using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Game.Advanture
{
    [AI(Name = "冒险对决",
        Description = "AI for Advanture Fight.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class AdvantureAi : AIBase
    {
        [EnterCommand(
            Command = "冒险对决",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "和一名成员进行冒险对决",
            Syntax = "[@QQ号]",
            SyntaxChecker = "At",
            Tag = "游戏功能",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 2)]
        public bool AdvantureAgainst(MsgInformationEx MsgDTO, object[] param)
        {
            var aimNum = (long) param[0];
            if (MsgDTO.FromQQ == aimNum)
            {
                MsgSender.PushMsg(MsgDTO, "你无法跟自己对决！");
                return false;
            }

            if (aimNum == Utility.SelfQQNum)
            {
                MsgSender.PushMsg(MsgDTO, "鱼唇的人类，你无法挑战ai的威严！");
                return false;
            }

            if (!AdvGameMgr.Instance.CheckGroup(MsgDTO.FromGroup))
            {
                MsgSender.PushMsg(MsgDTO, "本群正在进行一场对决，请稍后再试！");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO.FromGroup, aimNum, 
                $"{CodeApi.Code_At(MsgDTO.FromQQ)} 正在向你发起一场冒险对决，是否接受？", 10))
            {
                MsgSender.PushMsg(MsgDTO, "对决取消！");
                return false;
            }

            if (!AdvGameMgr.Instance.CheckPlayer(aimNum))
            {
                MsgSender.PushMsg(MsgDTO, "你的对手正在进行一场对决，请稍后再试！");
                return false;
            }

            if (!AdvGameMgr.Instance.CheckPlayer(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你正在进行一场对决，请稍后再试！");
                return false;
            }

            AdvGameMgr.Instance.GameStart(MsgDTO.FromGroup, MsgDTO.FromQQ, aimNum, 1);
            return true;
        }
    }
}
